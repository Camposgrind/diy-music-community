import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { MembersApiService } from './members-api.service';
import { MemberWriteRequest } from './models';

describe('MembersApiService', () => {
  let service: MembersApiService;
  let httpMock: HttpTestingController;
  const request: MemberWriteRequest = { name: 'Barney Greenway', instrument: 'Vocals', startYear: 1989, endYear: null, isCurrent: true, isLastKnownLineup: false };

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(MembersApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => httpMock.verify());

  it('should POST a member to its parent band', () => {
    service.createMember('band-1', request).subscribe();
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/bands/band-1/members`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush({ id: 'member-1' });
  });

  it('should PUT a member to update it', () => {
    service.updateMember('band-1', 'member-1', request).subscribe();
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/bands/band-1/members/member-1`);
    expect(req.request.method).toBe('PUT');
    req.flush({ id: 'member-1' });
  });

  it('should DELETE a member from its parent band', () => {
    service.deleteMember('band-1', 'member-1').subscribe();
    const req = httpMock.expectOne(`${environment.apiBaseUrl}/bands/band-1/members/member-1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
