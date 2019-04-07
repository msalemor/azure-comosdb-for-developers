import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

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


interface Contact {
  id: string,
  ContactType: string,
  LastName: string,
  FirstName: string,
  Email: string,
  Phone: string,
  Notes: string
}

interface DataItem {
  item1: number,
  item2: string,
  item3: string,
  item4: string,
  item5: Contact[]
}
