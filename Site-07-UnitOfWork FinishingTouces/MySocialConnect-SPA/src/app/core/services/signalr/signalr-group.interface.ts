export interface SignalRGroup{
  groupName: string;
  connections: SignalRConnection[];
}

export interface SignalRConnection{
  connectionId: string;
  userName: string;
  userId: number;
}
