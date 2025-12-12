import { useEffect, useState } from 'react'
import './App.css'

type User = {
  name: string
  email?: string
}

function App() {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadUser = async () => {
      try {
        setError(null)
        console.log('Checking auth status...')
        const res = await fetch('/api/auth/me', {
          credentials: 'include',
        })

        console.log('Auth response status:', res.status)
        console.log('Auth response headers:', Object.fromEntries(res.headers.entries()))

        if (res.status === 401) {
          console.log('User not authenticated')
          setUser(null)
          return
        }

        if (!res.ok) {
          throw new Error('Failed to load user')
        }

        const data = (await res.json()) as User
        console.log('User data received:', data)
        setUser(data)
      } catch (err) {
        console.error('Error checking auth:', err)
        setError(err instanceof Error ? err.message : 'Something went wrong')
      } finally {
        setLoading(false)
      }
    }

    loadUser()
  }, [])

  const login = () => {
    const returnUrl = encodeURIComponent(window.location.href)
    window.location.href = `/api/auth/login?returnUrl=${returnUrl}`
  }

  const logout = async () => {
    setError(null)
    setLoading(true)
    console.log('Logging out...')
    const res = await fetch('/api/auth/logout', {
      credentials: 'include',
    })
    console.log('Logout response:', res.status)
    setUser(null)
    setLoading(false)
  }

  return (
    <div className="page">
      <header>
        <h1>Auth Demo</h1>
        <p>Google sign-in via .NET backend</p>
      </header>

      <main>
        {loading && <p className="status">Checking session…</p>}
        {!loading && user && (
          <div className="panel">
            <p className="greeting">Welcome, {user.name}</p>
            {user.email && <p className="subtle">{user.email}</p>}
            <button className="secondary" onClick={logout}>
              Sign out
            </button>
          </div>
        )}

        {!loading && !user && (
          <div className="panel">
            <p className="greeting">You are not signed in.</p>
            <button onClick={login}>Sign in with Google</button>
          </div>
        )}

        {error && <p className="error">{error}</p>}
      </main>
    </div>
  )
}

export default App
