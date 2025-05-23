import axios, { AxiosError } from "axios";
import type { IAuthResponse } from "@models/account-models";
import { appStorage } from "./localstorage";
import { tokenManager } from "./token-manager"
import { useUserStore } from "@core/store/store";

export const apiBaseUrl = `https://192.168.0.100:5103`;
export const refreshUrl = `/accounts/getTokens/byRefresh`

export const accessTokenKey = 'accessToken'
export const refreshTokenKey = 'refreshToken'
export const roleKey = 'roleKey'

const $api = axios.create({ baseURL: apiBaseUrl })

$api.interceptors.request.use((config) => {
    const accessToken = appStorage.getItem(accessTokenKey);
    if (accessToken != null) {
        config.headers['X-Authorization'] = `Bearer ${accessToken}`
    }
    return config;
})
$api.interceptors.response.use(config => config, async (requestError) => {
    const originalRequest = requestError.config;
    if (!requestError.response) {
      console.error("Глобальный перехват: сервер недоступен");
      throw new AxiosError('Сервер не доступен, проверьте подключение');
    }
    if (requestError.response &&requestError.response.status == 401 && requestError.config && !requestError.config._isRetry) {
        originalRequest._isRetry = true;
        try {
            const refreshToken = appStorage.getItem(refreshTokenKey);
            console.log(refreshToken)
            const response = await $api.patch<IAuthResponse>(refreshUrl, {refreshToken});
            console.log(response);
            appStorage.setItem(accessTokenKey, response.data.accessToken);
            appStorage.setItem(refreshTokenKey, response.data.refreshToken);
            console.log('Refresh token');

            return $api.request(originalRequest);
        } catch (error) {
            if (error instanceof Error) {
                console.error("Ошибка:", error.message); 
            } else {
                console.error("Неизвестная ошибка:", error);
            }
            tokenManager.removeAuth();
            useUserStore.getState().logout()
            window.location.href = '/dashboard'
        }
    }
    throw requestError;
})
export default $api;