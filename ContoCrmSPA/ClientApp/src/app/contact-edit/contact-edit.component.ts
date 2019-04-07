import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Contact } from '../interfaces/Contact';
import { DataItem } from '../interfaces/DataItem';

@Component({
  selector: 'contact-edit',
  templateUrl: './contact-edit.component.html',
})
export class ContactEditComponent {
  public dataItems: DataItem;
  public contacts: Contact[];
  public contactType: string;
  httpClient: HttpClient;

  constructor(http: HttpClient, private route: ActivatedRoute, private router: Router) {
    this.httpClient = http;
    this.contactType = this.route.snapshot.paramMap.get("type");
    let serviceUri = 'https://localhost:44300/api/contacts/' + this.contactType;
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    http.get<DataItem>(serviceUri).subscribe(result => {
      this.contactType += 's';
      this.dataItems = result;
      this.contacts = result.item5;
    }, error => console.error(error));
  }

  submit(id: string) {
    if (confirm('Delete record ' + id + '?')) {
    }
  }
}
