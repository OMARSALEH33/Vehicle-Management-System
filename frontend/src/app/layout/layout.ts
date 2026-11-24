import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter, map } from 'rxjs';
import { AuthService } from '../services/authservice';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  styleUrl: './layout.css',
  templateUrl: './layout.html',
})
export class Layout {
  private readonly router = inject(Router);
  readonly authService = inject(AuthService);
  private readonly url = toSignal(
    this.router.events.pipe(
      filter((e) => e instanceof NavigationEnd),
      map(() => this.router.url)
    ),
    { initialValue: this.router.url }
  );

  title = computed(() =>
    this.url().startsWith('/vehicles')
      ? 'Vehicles'
      : this.url().startsWith('/setting/makes')
        ? 'Makes'
        : this.url().startsWith('/setting/models')
          ? 'Models'
          : this.url().startsWith('/setting')
            ? 'Setting'
            : 'Dashboard'
  );
}