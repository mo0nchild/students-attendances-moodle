class AppLocalStorage extends Object {

    public constructor(private readonly _storage: Storage) { 
        super(); 
    }
    
    protected storageEventFactory = (key: string, old: string | null, next: string | null)
        : StorageEvent => {
        return new StorageEvent('storage', {
            key: key,
            oldValue: old,
            newValue: next,
            url: window.location.href,
            storageArea: this._storage,
        })
    }
    public setItem(key: string, value: string): void {
        const oldValue = this._storage.getItem(key);
        this._storage.setItem(key, value);
        console.log(this._storage)
        window.dispatchEvent(this.storageEventFactory(key, oldValue, value));
    }
    public removeItem(key: string): void {
        const oldValue = this._storage.getItem(key);
        this._storage.removeItem(key);
        window.dispatchEvent(this.storageEventFactory(key, oldValue, null));
    }
    public getItem(key: string): string | null {
        return this._storage.getItem(key);
    }
}
export const appStorage = new AppLocalStorage(localStorage);