// store/slices/authSlice.ts
// store/slices/authSlice.ts
import type { ICredentialsInfo } from '@core/models/account-models';
import { tokenManager } from '@core/utils/token-manager';
import { accountService } from '@services/AccountService';
import type { StateCreator } from 'zustand';
import type { UserInfoModel } from '@core/models/user-models';
import { AxiosError } from 'axios';

export interface AccountState {
  user: UserInfoModel | null
  isLoading: boolean
  error?: string
  
}
export interface AccountAction {
  login: (credentials: ICredentialsInfo) => Promise<boolean>
  logout: () => void
}

const initialAccountState: AccountState = {
  user: null,
  isLoading: false,
  error: undefined
}

export type AccountSlice = AccountState & AccountAction

export const createAccountSlice: StateCreator<AccountSlice, [], [], AccountSlice> = (set) => ({
  login: async (credentials) => {
    try {
      set({ isLoading: true, error: undefined })
      const info = (await accountService.login(credentials)).data
      tokenManager.setAuth(info)
      if (info.role.toLocaleLowerCase() !== 'admin') {
        const userInfo = (await accountService.getInfo()).data
        set({ isLoading: false, user: userInfo })
      } 
      else set({ isLoading: false })
      return true
    }
    catch (error) {
      if (error instanceof AxiosError) {
        if (error.response) {
          set({ isLoading: false, error: 'Пользователь не найден' })
        } else {
          set({ isLoading: false, error: error.message })
        }
      }
      else set({ isLoading: false, error: 'Неизвестная ошибка' })
      console.log(error)
      return false
    }
  },
  logout: () => set({...initialAccountState}),
  ...initialAccountState
})
