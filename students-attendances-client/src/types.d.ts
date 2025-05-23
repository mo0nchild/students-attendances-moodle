// src/types.d.ts
export {};

declare global {
  interface Navigator {
    serial: {
      requestPort(options?: SerialPortRequestOptions): Promise<SerialPort>;
      getPorts(): Promise<SerialPort[]>;
    };
  }

  interface SerialPort {
    open(options: { baudRate: number }): Promise<void>;
    readable: ReadableStream<Uint8Array> | null;
    writable: WritableStream<Uint8Array> | null;
    close(): Promise<void>;
  }

  interface SerialPortRequestOptions {
    filters?: Array<{ usbVendorId?: number; usbProductId?: number }>;
  }


  declare class NDEFReader {
    constructor();
    scan(options?: { signal?: AbortSignal }): Promise<void>;
    write(message: string | NDEFMessageInit): Promise<void>;
    onreading: ((event: NDEFReadingEvent) => void) | null;
    onerror: ((event: Event) => void) | null;
  }

  interface NDEFReadingEvent extends Event {
    message: NDEFMessage;
    serialNumber: string;
  }

  interface NDEFMessage {
    records: NDEFRecord[];
  }

  interface NDEFRecord {
    recordType: string;
    mediaType?: string;
    id?: string;
    encoding?: string;
    lang?: string;
    data: DataView;
  }

  interface NDEFMessageInit {
    records: NDEFRecordInit[];
  }

  interface NDEFRecordInit {
    recordType: string;
    mediaType?: string;
    id?: string;
    encoding?: string;
    lang?: string;
    data?: BufferSource | string;
  }

}

