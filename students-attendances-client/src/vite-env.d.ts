/// <reference types="vite/client" />

declare module 'virtual:pwa-register' {
  export type RegisterSWOptions = {
    immediate?: boolean
    onNeedRefresh?: () => void
    onOfflineReady?: () => void
  }

  export function registerSW(options?: RegisterSWOptions): () => void
}

declare module 'virtual:pwa-register/react' {
  export function useRegisterSW(opts?: {
    immediate?: boolean
    onNeedRefresh?: () => void
    onOfflineReady?: () => void
    onRegisteredSW?: (swScriptUrl: string, registration: ServiceWorkerRegistration | undefined) => void
  }): {
    offlineReady: boolean
    needRefresh: boolean
    updateSW: () => Promise<void>
  }
}