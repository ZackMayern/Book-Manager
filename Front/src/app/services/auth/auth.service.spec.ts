import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthService);
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should set auth data and mark as authenticated', () => {
    const user = {
      id: 1,
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      password: '',
      isAdmin: false
    };

    service.setAuthData('test-token', user);

    expect(service.isLoggedIn()).toBe(true);
    expect(service.getToken()).toBe('test-token');
    expect(service.getCurrentUser()).toEqual(user);
  });

  it('should logout and clear auth data', () => {
    const user = {
      id: 1,
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      password: '',
      isAdmin: false
    };

    service.setAuthData('test-token', user);
    service.logout();

    expect(service.isLoggedIn()).toBe(false);
    expect(service.getToken()).toBeNull();
    expect(service.getCurrentUser()).toBeNull();
  });
});
