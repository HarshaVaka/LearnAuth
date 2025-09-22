import { API } from "./api";

export class HomeService {
  static checkAuth = async () => {
    const response = await API.get("/home/protect");
    return response.data;
  };
}