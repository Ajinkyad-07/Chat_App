export interface Message {
  Id: string;
  Text: string;
  ReceiverId?: string;
  ReceiverName: string;
  SenderId?: string;
  SenderName: string;
  Timestamp: Date;
}
