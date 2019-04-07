import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Contact } from '../interfaces/Contact';
import { ContactDetailsResult } from '../interfaces/ContactDefailsResult';

@Component({
  selector: 'contact-details',
  templateUrl: './contact-details.component.html',
})
export class ContactDetailsComponent {
  public result: ContactDetailsResult;
  public contact: Contact;
  public contactType: string;
  public contactTypes: string;
  httpClient: HttpClient;

  constructor(http: HttpClient, private route: ActivatedRoute, private router: Router) {
    this.httpClient = http;
    this.contactType = this.route.snapshot.paramMap.get("type");
    let id = this.route.snapshot.paramMap.get("id");
    let serviceUri = `https://localhost:44300/api/contacts/${id}/${this.contactType}`;
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    http.get<ContactDetailsResult>(serviceUri).subscribe(result => {
      this.contactTypes = this.contactType + 's';
      this.result = result;
      this.contact = result.item5;
    }, error => console.error(error));
  }
}
