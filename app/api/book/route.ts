import {NextResponse} from 'next/server'
import {readDB, writeDB} from '@/lib/store'
import {getSlots, istDate} from '@/lib/slots'
import {sendInvites} from '@/lib/mail'

const clean = (v: unknown, n = 1000) => String(v ?? '').trim().slice(0, n)

export async function POST(req: Request) {
  const b = await req.json().catch(() => null)
  if (!b || !['typeId', 'startAt', 'parentName', 'email', 'phone'].every((k) => clean(b[k])) || !/^\S+@\S+\.\S+$/.test(clean(b.email)))
    return NextResponse.json({error: 'Please fill in all required fields.'}, {status: 400})

  // Check-and-write is fully synchronous (no awaits), so two requests can't interleave in one server process.
  const db = readDB()
  const t = db.consultTypes.find((x) => x.id === b.typeId && x.active)
  const st = Date.parse(b.startAt)
  if (!t || isNaN(st)) return NextResponse.json({error: 'Invalid request.'}, {status: 400})

  const slot = getSlots(db, t, istDate(st)).find((s) => Date.parse(s.startAt) === st)
  if (!slot) return NextResponse.json({error: 'Sorry, that time was just taken. Please pick another.'}, {status: 409})

  const staffId = slot.staffIds[0]
  const paid = t.priceINR > 0
  const booking = {
    id: `booking-${staffId}-${st}`, typeId: t.id, staffId,
    startAt: new Date(st).toISOString(), endAt: new Date(st + t.durationMins * 60000).toISOString(),
    blockEnd: st + (t.durationMins + t.bufferMins) * 60000,
    status: paid ? 'pending_payment' : 'confirmed',
    parentName: clean(b.parentName, 120), email: clean(b.email, 200), phone: clean(b.phone, 30),
    childName: clean(b.childName, 120), childAge: clean(b.childAge, 30), concern: clean(b.concern, 2000),
    createdAt: new Date().toISOString(),
  }
  db.bookings.push(booking)
  writeDB(db)

  if (!paid) {
    try { await sendInvites(booking, t, db.staff.find((s) => s.id === staffId)!, 'confirmed') }
    catch (e) { console.error('[mail] failed', e) } // booking stays saved even if email fails
  }
  return NextResponse.json({ok: true, bookingId: booking.id, needsPayment: paid})
}
