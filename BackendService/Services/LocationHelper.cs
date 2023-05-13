namespace BackendService.Services;

public static class LocationHelper
{
    private const double delta = 10 ;
    private const double razaPamantului = 6371000;
    private const double pi = 3.14159265359;
    public static bool VerifyLocation(double lat1, double long1, double lat2, double long2){

        double distanceG = Math.Sqrt(Math.Pow(lat2 - lat1, 2) + Math.Pow(long2 - long1, 2));
        double distanceM = distanceG * pi /180*razaPamantului;
        return distanceM <= delta;

    }
}