import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { BandsResultsComponent } from './bands-results.component';
import { BandListItemModel, PagedResult } from '../../../infrastructure/api/models';

describe('BandsResultsComponent', () => {
  let fixture: ComponentFixture<BandsResultsComponent>;
  let componentRef: ComponentRef<BandsResultsComponent>;
  let el: HTMLElement;

  const mockBand: BandListItemModel = {
    id: '1',
    name: 'Discharge',
    country: 'UK',
    genre: 'D-Beat',
    status: 'Active',
    formationYear: 1977,
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BandsResultsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BandsResultsComponent);
    componentRef = fixture.componentRef;
    el = fixture.nativeElement;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should show loading spinner when loading is true', () => {
    componentRef.setInput('loading', true);
    fixture.detectChanges();
    expect(el.querySelector('.results__spinner')).toBeTruthy();
    expect(el.textContent).toContain('Searching the underground');
  });

  it('should show error message when error is set', () => {
    componentRef.setInput('error', 'Something went wrong');
    fixture.detectChanges();
    expect(el.querySelector('.results__error')).toBeTruthy();
    expect(el.textContent).toContain('Something went wrong');
  });

  it('should show empty state when results has zero items', () => {
    const emptyResult: PagedResult<BandListItemModel> = {
      items: [],
      page: 1,
      pageSize: 20,
      totalCount: 0,
    };
    componentRef.setInput('results', emptyResult);
    fixture.detectChanges();
    expect(el.querySelector('.results__empty')).toBeTruthy();
    expect(el.textContent).toContain('No bands match your search');
  });

  it('should show result count', () => {
    const result: PagedResult<BandListItemModel> = {
      items: [mockBand],
      page: 1,
      pageSize: 20,
      totalCount: 1,
    };
    componentRef.setInput('results', result);
    fixture.detectChanges();
    expect(el.textContent).toContain('1 band found');
  });

  it('should pluralize result count', () => {
    const result: PagedResult<BandListItemModel> = {
      items: [mockBand, { ...mockBand, id: '2', name: 'Napalm Death' }],
      page: 1,
      pageSize: 20,
      totalCount: 2,
    };
    componentRef.setInput('results', result);
    fixture.detectChanges();
    expect(el.textContent).toContain('2 bands found');
  });

  it('should render band cards for each item', () => {
    const result: PagedResult<BandListItemModel> = {
      items: [mockBand, { ...mockBand, id: '2', name: 'Napalm Death' }],
      page: 1,
      pageSize: 20,
      totalCount: 2,
    };
    componentRef.setInput('results', result);
    fixture.detectChanges();
    const cards = el.querySelectorAll('dmc-band-card');
    expect(cards.length).toBe(2);
  });

  it('should show pagination when multiple pages exist', () => {
    const result: PagedResult<BandListItemModel> = {
      items: [mockBand],
      page: 1,
      pageSize: 1,
      totalCount: 3,
    };
    componentRef.setInput('results', result);
    fixture.detectChanges();
    expect(el.querySelector('.results__pagination')).toBeTruthy();
  });

  it('should not show pagination for single page', () => {
    const result: PagedResult<BandListItemModel> = {
      items: [mockBand],
      page: 1,
      pageSize: 20,
      totalCount: 1,
    };
    componentRef.setInput('results', result);
    fixture.detectChanges();
    expect(el.querySelector('.results__pagination')).toBeFalsy();
  });

  it('should emit pageChange when page button is clicked', () => {
    const result: PagedResult<BandListItemModel> = {
      items: [mockBand],
      page: 1,
      pageSize: 1,
      totalCount: 3,
    };
    componentRef.setInput('results', result);
    fixture.detectChanges();

    let emittedPage: number | undefined;
    fixture.componentInstance.pageChange.subscribe((p: number) => (emittedPage = p));

    const nextBtn = el.querySelectorAll('.results__page-btn');
    const lastBtn = nextBtn[nextBtn.length - 1] as HTMLButtonElement;
    lastBtn.click();

    expect(emittedPage).toBe(2);
  });

  it('should disable Prev button on first page', () => {
    const result: PagedResult<BandListItemModel> = {
      items: [mockBand],
      page: 1,
      pageSize: 1,
      totalCount: 3,
    };
    componentRef.setInput('results', result);
    fixture.detectChanges();
    const prevBtn = el.querySelector('.results__page-btn') as HTMLButtonElement;
    expect(prevBtn.disabled).toBe(true);
  });
});
