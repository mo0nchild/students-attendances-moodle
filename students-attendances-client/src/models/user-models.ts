import type { GroupInfoModel } from "./group-models";

export interface UserInfoModel {
  externalId: number;
  createdTime: string;
  modifiedTime: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  city: string;
  country: string;
  description: string;
  imageUrl: string;
}

export interface UserInfoWithGroupsModel extends UserInfoModel {
  groups: GroupInfoModel[];
}

export interface RfidMarkerModel {
  uuid: string;
  userId: number;
  rfidValue: string;
}

export interface NewRfidMarkerModel {
  userId: number;
  rfidValue: string;
}

export interface RfidMarkerDetailsModel {
  uuid: string
  userId: number
  rfidValue: string
  user: UserInfoModel
}