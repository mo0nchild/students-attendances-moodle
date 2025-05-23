import type { GroupInfoModel } from "./group-models";
import type { UserInfoModel } from "./user-models";

export interface CourseInfoModel {
  externalId: number;
  createdTime: string;
  modifiedTime: string;
  shortName: string;
  fullName: string;
  format: string;
  startDate: string;
  endDate?: string | null;
  teachers: UserInfoModel[];
  groups: GroupInfoModel[];
  attendanceModules: AttendanceModuleInfo[];
}

export type GroupMode = "None" | "Isolated" | "Visible";

export interface AttendanceModuleInfo {
  externalId: number;
  name: string;
  groupMode: GroupMode;
}