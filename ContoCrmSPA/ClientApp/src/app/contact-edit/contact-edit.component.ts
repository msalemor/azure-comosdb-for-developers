import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Contact } from '../interfaces/Contact';
import { ContactDetailsResult } from '../interfaces/ContactDetailsResult';

let serviceUri = 'https://localhost:44300/api/contacts';

@Component({
  selector: 'contact-edit',
  templateUrl: './contact-edit.component.html',
})
export class ContactEditComponent {
  private result: ContactDetailsResult;
  public contact: Contact;
  public contactType: string;
  private id: string;

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router) {
    this.contactType = this.route.snapshot.paramMap.get("type");
    this.id = this.route.snapshot.paramMap.get("id");
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };

    // If editing load the contact
    if (this.id !== null) {
      let uri = serviceUri + `/${this.id}/${this.contactType}`;
      http.get<ContactDetailsResult>(uri).subscribe(result => {
        this.result = result;
        this.contact = result.item5;
      }, error => console.error(error));
    } else {
      this.contact = {
        id: null,
        contactType: this.contactType,
        company: null,
        lastName: null,
        firstName: null,
        email: null,
        phone: null,
        notes: null
      };
    }

  }

  submit() {
    if (this.contact.id === null) {
      // Creating
      this.contact.id = null;
      this.contact.contactType = this.contactType;
      this.http.post(serviceUri, this.contact).subscribe(
        result => {
          console.info("Record created");
          this.router.navigateByUrl(`/contact-list/${this.contactType}`);
        },
        error => {
          console.error(error);
        })
    } else {
      // editing
      let uri = `${serviceUri}/${this.id}`;
      this.http.put(uri, this.contact).subscribe(
        result => {
          console.info("Record updated");
          this.router.navigateByUrl(`/contact-list/${this.contactType}`);
        },
        error => {
          console.error(error)
        });
    }
  }
}
