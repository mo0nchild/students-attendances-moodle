import { useCallback, useEffect, useRef, useState } from 'react';

interface UseNfcOptions {
  enabled?: boolean;
  onData?: (data: string) => void;
  onError?: (error: Error) => void;
}

interface UseNfcState {
  isSupported: boolean;
  isScanning: boolean;
}

export function useNfc({ enabled = false, onData, onError }: UseNfcOptions = {}): UseNfcState {
  const [isSupported, setIsSupported] = useState<boolean>(false);
  const [isScanning, setIsScanning] = useState<boolean>(false);

  const readerRef = useRef<NDEFReader | null>(null);

  const startScan = useCallback(async () => {
    if (!('NDEFReader' in window)) {
      if (onError) onError(new Error('Web NFC API not supported'));
      return;
    }

    try {
      const reader = new NDEFReader();
      readerRef.current = reader;

      reader.onreading = (event: NDEFReadingEvent) => {
        new Audio("/nfcsound.mp3").play();
        for (const record of event.message.records) {
          if (record.recordType === 'text') {
            const textDecoder = new TextDecoder(record.encoding || 'utf-8');
            const text = textDecoder.decode(record.data);
            if (onData) onData(text);
          }
        }
      };

      reader.onerror = () => {
        if (onError) onError(new Error('NFC reading error'));
      };

      await reader.scan();
      setIsScanning(true);
    } catch (err) {
      if (onError) onError(err as Error);
    }
  }, [onData, onError]);

  const stopScan = useCallback(() => {
    if (readerRef.current) {

      readerRef.current.onreading = null;
      readerRef.current.onerror = null;
      readerRef.current = null;
    }
    setIsScanning(false);
  }, []);

  useEffect(() => {
    setIsSupported('NDEFReader' in window);
  }, []);

  useEffect(() => {
    if (enabled && !isScanning) {
      startScan();
    } else if (!enabled && isScanning) {
      stopScan();
    }
  }, [enabled, isScanning, startScan, stopScan]);

  return { isSupported, isScanning };
}
