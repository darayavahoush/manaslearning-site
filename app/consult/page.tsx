'use client'
import {useEffect, useState} from 'react'
import type {FormEvent} from 'react'

type T = {id: string; title: string; description: string; durationMins: number; priceINR: number}
type S = {startAt: string; label: string}
const ist = (ms: number) => new Date(ms + 19800000).toISOString().slice(0, 10)
const longDay = (d: string) => new Date(d + 'T00:00:00Z').toLocaleDateString('en-IN', {weekday: 'long', day: 'numeric', month: 'long', timeZone: 'UTC'})
const monthName = (m: string) => new Date(m + '-01T00:00:00Z').toLocaleDateString('en-IN', {month: 'long', year: 'numeric', timeZone: 'UTC'})
const shiftMonth = (m: string, n: number) => { const [y, mo] = m.split('-').map(Number); return new Date(Date.UTC(y, mo - 1 + n, 1)).toISOString().slice(0, 7) }
const t12 = (hm: string) => { const [h, m] = hm.split(':').map(Number); return `${h % 12 || 12}:${String(m).padStart(2, '0')} ${h < 12 ? 'AM' : 'PM'}` }
const input = 'w-full rounded-xl border border-cream-dark bg-white px-4 py-3 text-sm outline-none focus:border-primary'

export default function Consult() {
  const [types, setTypes] = useState<T[]>([])
  const [t, setT] = useState<T | null>(null)
  const [today, setToday] = useState('')
  const [month, setMonth] = useState('')
  const [avail, setAvail] = useState<Record<string, number>>({})
  const [date, setDate] = useState('')
  const [slots, setSlots] = useState<S[]>([])
  const [slot, setSlot] = useState<S | null>(null)
  const [f, setF] = useState({parentName: '', email: '', phone: '', childName: '', childAge: '', concern: ''})
  const [msg, setMsg] = useState('')
  const [done, setDone] = useState<null | {needsPayment: boolean}>(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    const td = ist(Date.now())
    setToday(td); setMonth(td.slice(0, 7))
    const q = new URLSearchParams(window.location.search)
    if (q.get('embed')) { const h = document.querySelector('header') as HTMLElement | null; if (h) h.style.display = 'none' }
    fetch('/api/types').then((r) => r.json()).then((list: T[]) => {
      setTypes(list)
      setT(list.find((x) => x.id === q.get('type')) || list.find((x) => x.id === 'free-counselling') || list[0] || null)
    })
  }, [])

  const loadAvail = () => { if (t && month) fetch(`/api/availability?type=${t.id}&month=${month}`).then((r) => r.json()).then(setAvail) }
  const loadSlots = () => { if (t && date) fetch(`/api/slots?type=${t.id}&date=${date}`).then((r) => r.json()).then(setSlots) }
  useEffect(() => { setDate(''); setSlots([]); setSlot(null); loadAvail() }, [t]) // eslint-disable-line
  useEffect(() => { loadAvail() }, [month]) // eslint-disable-line
  useEffect(() => { setSlot(null); loadSlots() }, [date]) // eslint-disable-line

  async function submit(e: FormEvent) {
    e.preventDefault()
    if (!t || !slot) return
    setBusy(true); setMsg('')
    const r = await fetch('/api/book', {method: 'POST', headers: {'Content-Type': 'application/json'}, body: JSON.stringify({...f, typeId: t.id, startAt: slot.startAt})})
    const j = await r.json()
    setBusy(false)
    if (r.ok) setDone({needsPayment: j.needsPayment})
    else { setMsg(j.error || 'Something went wrong.'); if (r.status === 409) { setSlot(null); loadSlots(); loadAvail() } }
  }

  if (done) return (
    <main className="mx-auto max-w-xl px-5 py-16 text-center">
      <h1 className="font-patrick text-4xl text-primary">{done.needsPayment ? 'Almost there!' : 'You’re booked!'}</h1>
      <p className="mt-4 text-text-brown">
        {done.needsPayment ? 'Your slot is held. The payment step is added in a later patch.' : `${t?.title} on ${longDay(date)} at ${slot ? t12(slot.label) : ''} IST. A confirmation email with a calendar invite is on its way.`}
      </p>
    </main>
  )

  const first = month ? new Date(month + '-01T00:00:00Z') : null
  const lead = first ? (first.getUTCDay() + 6) % 7 : 0
  const daysIn = month ? new Date(Date.UTC(Number(month.slice(0, 4)), Number(month.slice(5)), 0)).getUTCDate() : 0
  const cells: (string | null)[] = [...Array(lead).fill(null), ...Array.from({length: daysIn}, (_, i) => `${month}-${String(i + 1).padStart(2, '0')}`)]

  return (
    <main className="mx-auto max-w-4xl px-5 py-8">
      <h1 className="font-patrick text-4xl text-primary">Book a consultation</h1>
      <p className="mt-1 text-sm text-brown-light">Pick a day on the calendar, then a time. All times are India time (IST).</p>

      <div className="mt-5 flex flex-wrap gap-2">
        {types.map((x) => (
          <button key={x.id} onClick={() => setT(x)} className={`rounded-full px-4 py-2 text-sm font-medium ${t?.id === x.id ? 'bg-primary text-white' : 'bg-cream-dark text-text-brown hover:bg-white'}`}>
            {x.title} · {x.durationMins} min · {x.priceINR ? `₹${x.priceINR}` : 'Free'}
          </button>
        ))}
      </div>

      <div className="mt-6 grid gap-6 md:grid-cols-[1.2fr_1fr]">
        <div className="rounded-2xl bg-white p-5 shadow-sm">
          <div className="mb-3 flex items-center justify-between">
            <button disabled={month <= today.slice(0, 7)} onClick={() => setMonth(shiftMonth(month, -1))} className="rounded-full bg-cream-dark px-3 py-1 text-text-brown disabled:opacity-30">←</button>
            <div className="font-semibold text-text-brown">{month && monthName(month)}</div>
            <button onClick={() => setMonth(shiftMonth(month, 1))} className="rounded-full bg-cream-dark px-3 py-1 text-text-brown">→</button>
          </div>
          <div className="grid grid-cols-7 gap-1 text-center text-[11px] text-muted-gray">
            {['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'].map((d) => <div key={d}>{d}</div>)}
          </div>
          <div className="mt-1 grid grid-cols-7 gap-1">
            {cells.map((d, i) => {
              if (!d) return <div key={i} />
              const ok = !!avail[d]
              return (
                <button key={d} disabled={!ok} onClick={() => setDate(d)}
                  className={`aspect-square rounded-xl text-sm transition ${d === date ? 'bg-primary font-bold text-white' : ok ? 'bg-accent-green/60 font-medium text-text-brown hover:bg-accent-green' : 'text-muted-gray/60'} ${d === today && d !== date ? 'ring-2 ring-primary' : ''}`}>
                  {Number(d.slice(8))}
                </button>
              )
            })}
          </div>
          <div className="mt-3 flex gap-4 text-[11px] text-brown-light">
            <span className="flex items-center gap-1"><i className="inline-block h-3 w-3 rounded bg-accent-green" />Times available</span>
            <span className="flex items-center gap-1"><i className="inline-block h-3 w-3 rounded ring-2 ring-primary" />Today</span>
          </div>
        </div>

        <div className="rounded-2xl bg-white p-5 shadow-sm">
          <div className="font-semibold text-text-brown">{date ? longDay(date) : 'Select a day'}</div>
          <div className="mt-3 grid grid-cols-2 gap-2">
            {date && slots.length === 0 && <p className="col-span-2 text-sm text-muted-gray">No times left this day.</p>}
            {slots.map((s) => (
              <button key={s.startAt} onClick={() => setSlot(s)} className={`rounded-xl border px-3 py-2 text-sm ${slot?.startAt === s.startAt ? 'border-primary bg-primary text-white' : 'border-cream-dark bg-cream text-text-brown hover:border-primary'}`}>{t12(s.label)}</button>
            ))}
          </div>
        </div>
      </div>

      {t && slot && (
        <form onSubmit={submit} className="mt-6 grid gap-3 rounded-2xl bg-white p-5 shadow-sm sm:grid-cols-2">
          <input required className={input} placeholder="Your name *" value={f.parentName} onChange={(e) => setF({...f, parentName: e.target.value})} />
          <input required type="email" className={input} placeholder="Email *" value={f.email} onChange={(e) => setF({...f, email: e.target.value})} />
          <input required className={input} placeholder="Phone / WhatsApp *" value={f.phone} onChange={(e) => setF({...f, phone: e.target.value})} />
          <input className={input} placeholder="Child’s name" value={f.childName} onChange={(e) => setF({...f, childName: e.target.value})} />
          <input className={input} placeholder="Child’s age" value={f.childAge} onChange={(e) => setF({...f, childAge: e.target.value})} />
          <textarea className={`${input} sm:col-span-2`} rows={3} placeholder="What would you like help with?" value={f.concern} onChange={(e) => setF({...f, concern: e.target.value})} />
          {msg && <p className="text-sm text-card-red sm:col-span-2">{msg}</p>}
          <button disabled={busy} className="rounded-full bg-primary px-6 py-3 font-semibold text-white hover:bg-brand-orange disabled:opacity-60 sm:col-span-2">
            {busy ? 'Booking…' : `Confirm ${longDay(date)} at ${t12(slot.label)}`}
          </button>
        </form>
      )}
    </main>
  )
}
