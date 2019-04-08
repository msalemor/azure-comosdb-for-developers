import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DataItem } from '../interfaces/DataItem';
//import { Contact } from '../interfaces/Contact';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public result: DataItem;

  constructor(http: HttpClient) {
  let serviceUri = "https://localhost:44300/api/contacts";
    http.get<DataItem>(serviceUri).subscribe(result => {
      this.result = result;
    }, error => console.error(error));
  }
}
