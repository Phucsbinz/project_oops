# Sales Bicycle Store Demo

Đây là một dự án Console Application viết bằng **.NET 9.0**, mô phỏng hệ thống quản lý bán hàng cho một cửa hàng xe đạp (Sales Bicycle Store). 

Dự án thể hiện các khía cạnh cơ bản của một hệ thống thương mại điện tử / bán lẻ, bao gồm:
- Quản lý sản phẩm (Xe đạp các loại, vật liệu, giá cả).
- Quản lý khách hàng (Thông tin, Hạng thành viên, Tích điểm).
- Xử lý đơn hàng (Mua hàng, Áp dụng chiết khấu, Tính thuế).
- Hệ thống sự kiện (Events): Lắng nghe trạng thái đơn hàng, cảnh báo hết hàng, tích điểm, giao hàng, thông báo đổi vật liệu xe,...

## Yêu cầu hệ thống

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (Nếu chạy local)
- [Docker](https://www.docker.com/) (Nếu chạy qua container)

## Cách chạy dự án

### 1. Chạy trực tiếp qua .NET CLI (Local)

Mở terminal tại thư mục gốc của dự án (`d:\SalesBicycleStore`) và chạy lệnh sau:

```bash
dotnet run --project SalesBicycleStore/SalesBicycleStore.csproj
```

Dự án sẽ tự động biên dịch và hiển thị luồng chạy mô phỏng (demo) trực tiếp trên màn hình console.

### 2. Chạy qua Docker

Bạn có thể chạy dự án thông qua Docker bằng cách tự build hoặc kéo (pull) image trực tiếp từ Docker Hub.

#### A. Kéo Image từ Docker Hub (Khuyên dùng)
Dự án đã được thiết lập tự động push lên Docker Hub tại: [phucbinz/salesbicyclestore](https://hub.docker.com/repository/docker/phucbinz/salesbicyclestore/general)

**Bước 1: Kéo image về máy**
```bash
docker pull phucbinz/salesbicyclestore:latest
```

**Bước 2: Chạy Docker Container**
```bash
docker run --rm phucbinz/salesbicyclestore:latest
```

#### B. Tự Build Image Locallly (Dành cho nhà phát triển)
Tại thư mục gốc của dự án (nơi chứa file `dockerfile`):

**Bước 1: Build Docker Image**
```bash
docker build -t sales-bicycle-store .
```

**Bước 2: Chạy Docker Container**
```bash
docker run --rm sales-bicycle-store
```

*Lưu ý: Flag `--rm` sẽ tự động xóa container sau khi ứng dụng console chạy xong quá trình demo và thoát.*


## Cấu trúc thư mục chính
- **Domain**: Chứa các Entity cơ bản (Sản phẩm, Khách hàng, Đơn hàng,...).
- **Generics**: Chứa các lớp Generic dùng chung (VD: InMemoryRepository phục vụ việc lưu trữ dữ liệu tạm thời).
- **Pricing**: Chứa các interface và logic liên quan đến giá, chiết khấu và thuế.
- **Services**: Chứa logic nghiệp vụ chính như `OrderService` và `InventoryService` xử lý đơn hàng và tồn kho.
- **Program.cs**: Điểm khởi chạy của ứng dụng, chứa toàn bộ kịch bản (scenario) demo.
 