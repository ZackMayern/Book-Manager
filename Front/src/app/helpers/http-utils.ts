import { HttpParams } from '@angular/common/http';

export function createHttpParams(params: { [key: string]: any }): HttpParams {
  let httpParams = new HttpParams();
  for (const key in params) {
    if (params.hasOwnProperty(key)) {
      httpParams = httpParams.set(key, params[key]);
    }
  }
  return httpParams;
}