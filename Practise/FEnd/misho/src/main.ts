import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app-module';
import { Headercomponent } from './app/component/headercomponent/headercomponent';

platformBrowser().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
})
  .catch(err => console.error(err));
