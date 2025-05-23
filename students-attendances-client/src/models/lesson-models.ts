import type { GroupInfoModel } from "./group-models";

// Информация о посещении на занятии
export interface AttendanceInfoModel {
    acronym: string;
    description: string;
    remarks: string;
    studentId: number;
}

// Информация о занятии
export interface LessonInfoModel {
    externalId: number;
    createdTime: string;
    modifiedTime: string;
    description: string;
    startTime: string;
    endTime: string;
    version: number;
    attendanceId: number;
    groupInfo?: GroupInfoModel;
    attendances?: AttendanceInfoModel[];
}

// Создание занятия
export interface CreateLessonModel {
    description: string;
    startTime: string;
    endTime: string;
    attendanceId: number;
    courseId: number;
    groupId?: number | null;
}

export interface UpdateLessonRequest {
  lessonId: number;
  description: string;
  startTime: string;
  endTime: string;
}

// Модель для запроса взятия посещаемости
export interface AttendanceTakenRequest {
    lessonId: number;
    items: {
        studentId: number;
        acronym: string;
    }[];
}

export type SyncStatus = "Processing" | "LocalSaved" | "FullSync" | "Failed";
export type ActionType = "Create" | "Update" | "Delete";

export interface LessonSyncItem {
    externalId?: number | null
    action: ActionType;
    queuedAt: string;
    status: SyncStatus;
    errorMessage?: string | null;
    uuid: string;
    createdTime: string;
    modifiedTime: string;
}