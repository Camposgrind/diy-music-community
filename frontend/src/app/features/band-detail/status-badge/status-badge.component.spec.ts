import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { StatusBadgeComponent } from './status-badge.component';

describe('StatusBadgeComponent', () => {
  let fixture: ComponentFixture<StatusBadgeComponent>;
  let componentRef: ComponentRef<StatusBadgeComponent>;
  let el: HTMLElement;

  const setup = async (status: string) => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [StatusBadgeComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(StatusBadgeComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('status', status);
    fixture.detectChanges();
    el = fixture.nativeElement;
  };

  it('should create', async () => {
    await setup('Active');
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should display "Active" label unchanged', async () => {
    await setup('Active');
    expect(fixture.componentInstance.label()).toBe('Active');
  });

  it('should display "Split-Up" label for SplitUp status', async () => {
    await setup('SplitUp');
    expect(fixture.componentInstance.label()).toBe('Split-Up');
  });

  it('should display "On Hold" label for OnHold status', async () => {
    await setup('OnHold');
    expect(fixture.componentInstance.label()).toBe('On Hold');
  });

  it('should return "active" modifier for Active status', async () => {
    await setup('Active');
    expect(fixture.componentInstance.modifier()).toBe('active');
  });

  it('should return "splitup" modifier for SplitUp status', async () => {
    await setup('SplitUp');
    expect(fixture.componentInstance.modifier()).toBe('splitup');
  });

  it('should return "onhold" modifier for OnHold status', async () => {
    await setup('OnHold');
    expect(fixture.componentInstance.modifier()).toBe('onhold');
  });

  it('should return "unknown" modifier for an unrecognised status', async () => {
    await setup('Disbanded');
    expect(fixture.componentInstance.modifier()).toBe('unknown');
  });

  it('should pass through an unrecognised status as the label', async () => {
    await setup('Disbanded');
    expect(fixture.componentInstance.label()).toBe('Disbanded');
  });
});
