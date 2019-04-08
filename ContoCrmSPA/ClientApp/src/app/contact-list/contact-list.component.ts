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

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router) {
    this.contactType = route.snapshot.paramMap.get("type");
    let serviceUri = 'https://localhost:44300/api/contacts/' + this.contactType;
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    http.get<DataItem>(serviceUri).subscribe(result => {
      this.contactTypes = this.contactType + 's';
      this.result = result;
    }, error => console.error(error));
  }

  deleteContact(id: string, firstName: string, lastName: string) {
    let serviceUri = `https://localhost:44300/api/contacts/${id}`;
    if (confirm(`Are you sure you want to delete ${firstName} ${lastName}?`)) {
      this.http.delete(serviceUri).subscribe(
        result => {
          //TODO: Navigate to area
          this.router.navigateByUrl(`/contact-list/${this.contactType}`);
        },
        error => { console.error(error); });
    }
  }
}
