import type {Metadata} from 'next'
import {Poppins, Patrick_Hand} from 'next/font/google'
import './globals.css'

const poppins = Poppins({subsets: ['latin'], weight: ['400', '500', '600', '700'], variable: '--font-poppins'})
const patrick = Patrick_Hand({subsets: ['latin'], weight: '400', variable: '--font-patrick'})

export const metadata: Metadata = {title: 'Book a Consultation - Manas Learning'}

// TODO: replace with the real values from UrlConstants.cs
const BASE = 'https://www.manaslearning.com'
const SITE = {
  home: BASE,
  about: `${BASE}/about-us`,
  services: `${BASE}/services`,
  assessments: `${BASE}/assessments`,
  blog: `${BASE}/blog`,
  contact: `${BASE}/contact-us`,
  enquiry: `${BASE}/online-enquiry`,
  terms: `${BASE}/terms-of-service`,
  privacy: `${BASE}/privacy-policy`,
  youtube: '#', facebook: '#', instagram: '#', twitter: '#', linkedin: '#',
}

const NAV = [
  ['Services', SITE.services],
  ['Assessments', SITE.assessments],
  ['Blog', SITE.blog],
  ['Contact', SITE.contact],
] as const

export default function RootLayout({children}: {children: React.ReactNode}) {
  return (
    <html lang="en">
      <body className={`${poppins.variable} ${patrick.variable} antialiased`}>
        <header className="sticky top-0 z-40 border-b border-cream-dark bg-white/95 backdrop-blur">
          <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-5 py-3">
            <a href={SITE.home}><img src="/logo.png" alt="Manas Learning" width={200} className="h-auto w-[150px] sm:w-[200px]" /></a>
            <nav className="hidden items-center gap-7 text-sm font-medium text-text-brown lg:flex">
              {NAV.map(([label, href]) => <a key={label} href={href} className="hover:text-primary">{label}</a>)}
              <a href={SITE.enquiry} className="rounded-full bg-primary px-5 py-2 text-white hover:bg-brand-orange">Online Enquiry</a>
            </nav>
            <a href={SITE.home} className="text-sm text-text-brown hover:text-primary lg:hidden">← Site</a>
          </div>
        </header>

        {children}

        <footer className="mt-16 border-t border-cream-dark bg-cream">
          <div className="mx-auto max-w-5xl px-5 py-10 text-center">
            <a href={SITE.home}><img src="/logo.png" alt="Manas Learning" width={200} className="mx-auto h-auto" /></a>
            <ul className="mt-6 flex flex-wrap justify-center gap-x-7 gap-y-2 text-sm text-text-brown">
              <li><a href={SITE.about} className="hover:text-primary">About MANAS?</a></li>
              {NAV.map(([label, href]) => <li key={label}><a href={href} className="hover:text-primary">{label}</a></li>)}
              <li><a href={SITE.youtube} className="hover:text-primary">Videos</a></li>
            </ul>
            <ul className="mt-4 flex justify-center gap-5 text-sm text-brown-light">
              <li><a href={SITE.facebook}>Facebook</a></li>
              <li><a href={SITE.instagram}>Instagram</a></li>
              <li><a href={SITE.twitter}>Twitter</a></li>
              <li><a href={SITE.linkedin}>LinkedIn</a></li>
            </ul>
            <div className="mt-8 space-y-2 text-xs text-brown-light">
              <p className="font-semibold">© 2026 MANAS Learning. All rights reserved.</p>
              <p>Use of this website is subject to our <a className="underline" href={SITE.terms}>Terms of Use</a> and <a className="underline" href={SITE.privacy}>Privacy Policy</a>.</p>
              <p className="font-semibold">Sapphire, Olympia Opaline, Navalur, OMR, Chennai, Tamilnadu, India - 600103.</p>
              <p>Enquiry: <a className="underline" href="mailto:support@manaslearning.com">support@manaslearning.com</a> or <a className="underline" href="tel:+919342157217">(+91) 934-215-7217</a></p>
            </div>
          </div>
        </footer>
      </body>
    </html>
  )
}
