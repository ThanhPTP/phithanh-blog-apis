FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish "sources/PhiThanh.WebApi/PhiThanh.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8000

ENV ASPNETCORE_URLS=http://+:8000
COPY --from=build /app/publish .


# timezone env with default
ENV TZ="Asia/Ho_Chi_Minh"

ENTRYPOINT ["dotnet", "PhiThanh.WebApi.dll"]
