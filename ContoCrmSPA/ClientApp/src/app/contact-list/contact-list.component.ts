import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Contact } from '../interfaces/Contact';
import { DataItem } from '../interfaces/DataItem';

@Component({
  selector: 'contact-list',
  templateUrl: './contact-list.component.html',
})
export class ContactListComponent {
  public result: DataItem;
  public contacts: Contact[];
  public contactType: string;
  public contactTypes: string;
  httpClient: HttpClient;

  constructor(http: HttpClient, private route: ActivatedRoute, private router: Router) {
    this.httpClient = http;
    this.contactType = this.route.snapshot.paramMap.get("type");
    let serviceUri = 'https://localhost:44300/api/contacts/' + this.contactType;
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    http.get<DataItem>(serviceUri).subscribe(result => {
      this.contactTypes = this.contactType + 's';
      this.result = result;
      this.contacts = result.item5;
    }, error => console.error(error));
  }

  deleteContact(id: string) {
    if (confirm('Delete record ' + id + '?')) {
    }
  }
}
