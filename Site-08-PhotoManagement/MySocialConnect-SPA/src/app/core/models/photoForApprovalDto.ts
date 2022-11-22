export interface PhotoForApprovalDto{
  id: number;
  url: string;
  username: string;
  userId: number;
  userGuid: string;
  isApproved: boolean;
}
