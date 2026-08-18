export interface MemberWriteRequest {
  name: string;
  instrument: string | null;
  startYear: number | null;
  endYear: number | null;
  isCurrent: boolean;
  isLastKnownLineup: boolean;
}
