import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/user.model';

export const roleGuard = (roles: UserRole[]): CanActivateFn => {
  return () => {
    const user = inject(AuthService).currentUser();
    const router = inject(Router);

    if (user && roles.includes(user.role)) return true;

    return router.createUrlTree(['/dashboard']);
  };
};
