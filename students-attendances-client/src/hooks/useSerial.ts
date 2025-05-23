import { useCallback, useEffect, useRef, useState } from 'react';

interface UseSerialOptions {
  baudRate?: number;
  enabled?: boolean;
  onData?: (data: string) => void;
  onError?: (error: Error) => void;
}

interface UseSerialState {
  isSupported: boolean;
  isConnected: boolean;
}

export function useSerial({ baudRate = 9600, enabled = false, onData, onError }: UseSerialOptions = {}): UseSerialState {
  const [isSupported, setIsSupported] = useState<boolean>(false);
  const [isConnected, setIsConnected] = useState<boolean>(false);

  const portRef = useRef<SerialPort | null>(null);
  const readerRef = useRef<ReadableStreamDefaultReader<string> | null>(null);
  const readableStreamClosedRef = useRef<Promise<void> | null>(null);
  const textDecoderRef = useRef<TextDecoderStream | null>(null);
  const bufferRef = useRef<string>('');
  const stopSignalRef = useRef<boolean>(false);

  const readLoop = useCallback(async () => {
    if (!portRef.current) return;
    try {
      const textDecoder = new TextDecoderStream();
      textDecoderRef.current = textDecoder;
      readableStreamClosedRef.current = portRef.current.readable!.pipeTo(textDecoder.writable);
      readerRef.current = textDecoder.readable.getReader();

      while (true) {
				if (stopSignalRef.current) break;
				const { value, done } = await readerRef.current.read();
				if (done) break;
        if (value) {
          bufferRef.current += value;

          const lines = bufferRef.current.split('\n');
          bufferRef.current = lines.pop() || '';

          for (const line of lines) {
            if (onData) onData(line.trim().split(':')[1].trim());
          }
        }
      }

      await readableStreamClosedRef.current;
    } catch (err) {
      if (onError) onError(err as Error);
    }
  }, [onData, onError]);

  const connect = useCallback(async () => {
    try {
      if (!navigator.serial) throw new Error('Web Serial API not supported');
      const selectedPort = await navigator.serial.requestPort();
      await selectedPort.open({ baudRate });
      portRef.current = selectedPort;
      setIsConnected(true);
      stopSignalRef.current = false;
      bufferRef.current = '';
      readLoop();
    } catch (err) {
      if (onError) onError(err as Error);
    }
  }, [baudRate, readLoop, onError]);

  const disconnect = useCallback(async () => {
		stopSignalRef.current = true;
		bufferRef.current = '';
		console.log('Disconnect start');

		try {
			if (readerRef.current) {
				console.log('Cancelling reader');
				await readerRef.current.cancel().catch(() => {});
				readerRef.current.releaseLock();
				readerRef.current = null;
			}

			if (textDecoderRef.current) {
				console.log('Cancelling textDecoder');
				await textDecoderRef.current.readable.cancel().catch(() => {});
				textDecoderRef.current = null;
			}

			if (portRef.current) {
				if (portRef.current.readable?.locked) {
					console.log('Waiting for readableStreamClosed');
					await readableStreamClosedRef.current?.catch(() => {});
				}
				console.log('Closing port');
				await portRef.current.close();
				portRef.current = null;
			}
		} catch (e) {
			console.warn('Error during disconnect', e);
		}

		setIsConnected(false);
		console.log('Disconnected, isConnected=false');
	}, []);

  useEffect(() => {
    setIsSupported('serial' in navigator);
	}, []);

  useEffect(() => {
    if (enabled && !isConnected) {
        connect();
    } else if (!enabled && isConnected) {
        disconnect();
    }
	}, [enabled, isConnected, connect, disconnect]);


  return { isSupported, isConnected };
}
