import type { NewRfidMarkerModel, RfidMarkerDetailsModel, UserInfoModel } from "@core/models/user-models";
import $api from "@core/utils/api";
import type { AxiosResponse } from "axios";

class AdminService {

    public async getAllRfidMarkers(): Promise<AxiosResponse<RfidMarkerDetailsModel[]>> {
        return await $api.get(`/rfidmarkers/list/details`)
    }

    public async getUsersWithoutRfidMarkers(): Promise<AxiosResponse<UserInfoModel[]>> {
        return await $api.get(`/rfidmarkers/list/user/without`)
    }

    public async setRfidMarkerToStudent(rfidMarker: NewRfidMarkerModel): Promise<AxiosResponse<string>> {
        return await $api.post(`/rfidmarkers/set/marker`, rfidMarker)
    }

    public async deleteRfidMarker(uuid: string): Promise<AxiosResponse<string>> {
        return await $api.delete(`/rfidmarkers/delete`, { params: { 'uuid': uuid } })
    }

}
export const adminService = new AdminService()