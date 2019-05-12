import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AppConfigService {

  private appConfig: any;

  constructor(private http: HttpClient) { }

  loadAppConfig() {
    return this.http.get('appsettings.json')
      .toPromise()
      .then(data => {
        this.appConfig = data;
      });
  }

  getServiceApi() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }
    return this.appConfig.serviceApi;
  }

}
