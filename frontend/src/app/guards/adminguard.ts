import { inject } from "@angular/core"
import { Router } from "@angular/router";

export const adminGuard = () => {
    const router = inject(Router);
    const token = localStorage.getItem('auth_token');
if (!token){
    router.navigate(['/login']);
    return false;
}
const payload = JSON.parse(atob(token.split('.')[1]));

if (payload.role === 'Admin' || payload.role === 'SuperAdmin'){
    return  true;
}
alert('You are not authorized to enter')
router.navigate(['/'])
return false;
};