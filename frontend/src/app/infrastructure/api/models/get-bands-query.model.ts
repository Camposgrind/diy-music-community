export interface GetBandsQuery {
  name?: string;
  country?: string;
  genreId?: string;
  status?: string;
  page: number;
  pageSize: number;
}
