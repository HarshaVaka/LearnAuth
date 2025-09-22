import { useQuery } from "@tanstack/react-query";
import { HomeService } from "../services/HomeService";
import { useEffect } from "react";

export default function Home(){
    const {data, isFetching, isError,error,isSuccess} = useQuery({
        queryKey: ["checkAuth"],
        queryFn: () => HomeService.checkAuth(),
        staleTime: Infinity,
    });

    useEffect(() => {
        console.log({data, isFetching, isError,isSuccess,error});
    }, [data, isFetching, isError,isSuccess,error]);


    if(isError) return <div>Error occurred. Please try again.</div>
    if(isSuccess) return <div>Welcome to the protected Home Page!</div>
    return <></>
}