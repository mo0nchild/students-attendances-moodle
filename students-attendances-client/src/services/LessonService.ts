import type { AttendanceTakenRequest, CreateLessonModel, LessonInfoModel, LessonSyncItem, UpdateLessonRequest } from "@models/lesson-models"
import $api from "@utils/api"
import type { AxiosResponse } from "axios"

class LessonService {
    public async getLessonsList(courseId: number): Promise<AxiosResponse<LessonInfoModel[]>> {
        return await $api.get(`/lessons/list`, { params: { courseId } })
    }

    public async checkProcessing(processUuid: string): Promise<AxiosResponse<LessonSyncItem>> {
        return await $api.get(`/lessons/get/process`, { params: { processUuid } })
    }

    public async createLesson(info: CreateLessonModel): Promise<AxiosResponse<string>> {
        return await $api.post(`/lessons/create`, info)
    }

    public async updateLesson(info: UpdateLessonRequest): Promise<AxiosResponse<string>> {
        return await $api.patch(`/lessons/update`, info)
    }

    public async deleteLesson(lessonId: number): Promise<AxiosResponse<string>> {
        return await $api.delete(`/lessons/delete`, { params: { lessonId } })
    }

    public async setAttendances(info: AttendanceTakenRequest): Promise<AxiosResponse<string>> {
        return await $api.patch(`/lessons/update/attendance`, info)
    }
}

export const lessonService = new LessonService()