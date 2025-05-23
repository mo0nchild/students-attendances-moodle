import type { CourseInfoModel } from "@models/course-models"
import type { RfidMarkerModel, UserInfoWithGroupsModel } from "@models/user-models"
import $api from "@utils/api"
import type { AxiosResponse } from "axios"

class CoursesService {
    public async getCoursesList(): Promise<AxiosResponse<CourseInfoModel[]>> {
        return await $api.get(`/courses/list`)
    }
    public async getStudentsList(courseId: number): Promise<AxiosResponse<UserInfoWithGroupsModel[]>> {
        return await $api.get(`/courses/get/students`, { params: { courseId } })
    }
    public async getRfidMarkers(courseId: number): Promise<AxiosResponse<RfidMarkerModel[]>> {
        return await $api.get(`rfidmarkers/list/by/course`, { params: { courseId } })
    }
}
export const coursesService = new CoursesService()