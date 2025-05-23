import { useMemo } from 'react';
import { useSerial } from './useSerial';
import { useNfc } from './useNfc';

type DeviceType = 'serial' | 'nfc';

interface UseDeviceCommOptions {
  type?: DeviceType;
  enabled?: boolean;
  onData?: (data: string) => void;
  onError?: (error: Error) => void;
}

interface UseDeviceCommState {
  type: DeviceType | null;
  isSupported: boolean;
  isConnected: boolean;
}

function autoDetectDevice(): DeviceType | null {
  if ('serial' in navigator) return 'serial';
  if ('NDEFReader' in window) return 'nfc';
  return null;
}

export function useDeviceComm({
  type,
  enabled = false,
  onData,
  onError
}: UseDeviceCommOptions): UseDeviceCommState {
  const actualType = useMemo(() => type ?? autoDetectDevice(), [type]);

  const serial = useSerial({
    enabled: actualType === 'serial' && enabled,
    onData,
    onError
  });

  const nfc = useNfc({
    enabled: actualType === 'nfc' && enabled,
    onData,
    onError
  });

  const isSupported = actualType === 'serial'
    ? serial.isSupported
    : actualType === 'nfc' ? nfc.isSupported : false;

  const isConnected = actualType === 'serial'
    ? serial.isConnected
    : actualType === 'nfc' ? nfc.isScanning : false;

  return { type: actualType, isSupported, isConnected };
}
