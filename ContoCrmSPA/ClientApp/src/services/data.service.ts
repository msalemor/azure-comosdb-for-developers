import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Injectable()
export class DataService {
  private serviceUri: string = '';
  constructor(private http: HttpClient) {

  }
  getContactsByType(contactType: string) {
    let serviceUri = 'https://localhost:44300/api/contacts/' + contactType;
    return this.http.get(serviceUri);
  }

}
