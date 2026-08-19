import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GlobalLoaderComponent } from './global-loader.component';
import { LoadingService } from './loading.service';

describe('GlobalLoaderComponent', () => {
  let fixture: ComponentFixture<GlobalLoaderComponent>;
  let loading: LoadingService;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [GlobalLoaderComponent] });
    fixture = TestBed.createComponent(GlobalLoaderComponent);
    loading = TestBed.inject(LoadingService);
  });

  it('should use an absolute asset URL so the GIF loads from every route', () => {
    loading.begin();
    fixture.detectChanges();

    const image = fixture.nativeElement.querySelector('img') as HTMLImageElement;

    expect(image.getAttribute('src')).toBe('/images/loading.gif?v=2');
  });

});
