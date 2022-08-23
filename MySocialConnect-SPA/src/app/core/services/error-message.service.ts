import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ErrorMessageService {

  constructor() { }

  getHttpErrorMessage(error: any) {
    let msg = "";

    if (error.error.MessageDetail)
      msg = error.error.MessageDetail;
    else if (error.error.Message)
      msg = error.error.Message;
    else if (error.error && error.error === "System.Web.Http.ModelBinding.ModelStateDictionary")
      msg = "Please check data and try submitting again!";
    else if (error.error)
      msg = error.error; 
    else if (error == "System.Web.Http.ModelBinding.ModelStateDictionary")
      msg = "Please check data and try submitting again!"; 
    else if (error)
      msg = error; 

    return msg;
  }
}
