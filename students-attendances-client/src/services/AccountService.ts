
import type { IAuthResponse, ICredentialsInfo } from "@models/account-models";
import type { UserInfoModel } from "@models/user-models";
import $api from "@utils/api";
import type { AxiosResponse } from "axios";

class AccountService {
    public async login(credentials: ICredentialsInfo): Promise<AxiosResponse<IAuthResponse>> {
        return await $api.get(`/accounts/getTokens`, { params: credentials })
    }

     public async getInfo(): Promise<AxiosResponse<UserInfoModel>> {
        return await $api.get(`/users/info`)
    }
}
export const accountService = new AccountService()