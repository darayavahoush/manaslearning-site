import type {Metadata} from 'next'
import {Poppins, Patrick_Hand} from 'next/font/google'
import './globals.css'

const poppins = Poppins({subsets: ['latin'], weight: ['400', '500', '600', '700'], variable: '--font-poppins'})
const patrick = Patrick_Hand({subsets: ['latin'], weight: '400', variable: '--font-patrick'})

export const metadata: Metadata = {title: 'Book a Consultation - Manas Learning'}

export default function RootLayout({children}: {children: React.ReactNode}) {
  return (
    <html lang="en">
      <body className={`${poppins.variable} ${patrick.variable} antialiased`}>
        <header className="border-b border-cream-dark bg-cream">
          <div className="mx-auto flex max-w-5xl items-center justify-between px-5 py-4">
            <a href="https://www.manaslearning.com" className="font-patrick text-3xl text-primary">Manas Learning</a>
            <a href="https://www.manaslearning.com" className="text-sm text-text-brown hover:text-primary">← Back to site</a>
          </div>
        </header>
        {children}
      </body>
    </html>
  )
}
