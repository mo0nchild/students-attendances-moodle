import { useState, type JSX } from 'react'
import { RouterProvider } from 'react-router-dom';
import { routers } from '@utils/routers';
import Header from '@components/header/Header';
import WaveBackground from '@components/background/WaveBackground';

import { useRegisterSW } from 'virtual:pwa-register/react'

import 'bootstrap/dist/css/bootstrap.min.css'
import '@core/App.css'

const usePwaUpdatePrompt = () => {
  const [showPrompt, setShowPrompt] = useState(false)
  const [updateServiceWorker, setUpdateServiceWorker] = useState<() => void>(() => () => {})

  const { updateSW } = useRegisterSW({
    onNeedRefresh() {
      setShowPrompt(true)
      setUpdateServiceWorker(() => updateSW)
    },
    onOfflineReady() {
      console.log('PWA ready to work offline')
    },
  })

  return { showPrompt, updateServiceWorker }
}

export default function App(): JSX.Element {

  return (
    <div className="d-flex flex-column vh-100 overflow-hidden position-relative">
      <WaveBackground />

      {/* header — фиксирован */}
      <header className="flex-shrink-0">
        <Header />
      </header>

      {/* скроллимый main + footer */}
      <main className="flex-grow-1 overflow-auto position-relative" style={{ zIndex: 10, minHeight: 0 }}>
        <div className="d-flex flex-column" style={{ minHeight: '100%' }}>
          <div className="flex-grow-1 d-flex align-items-center justify-content-center">
            <RouterProvider router={routers} />
          </div>

          {/* footer внутри скролла */}
          <footer className="text-center p-3 bg-white bg-opacity-75 shadow-sm position-relative">
            <p className="text-dark mb-0">Связаться с нами: daniltulenev26@gmail.com</p>
            <p className="text-dark mb-0">
              <span style={{ color: '#ff6f61' }}>Исходный код проекта </span>
              <a style={{ color: '#6a11cb' }}>GitHub</a>
            </p>
          </footer>
        </div>
      </main>
    </div>
  );
}
