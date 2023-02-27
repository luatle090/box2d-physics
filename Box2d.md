# Box2d

### Hệ tọa độ của box2d

Box2d sử dụng hệ tọa độ Descartes theo góc bottom-left với x-right là hướng chiều dương, y-top là hướng chiều dương

Với đơn vị trong box2d là có thể là mét, kilogram, giây. Góc sử dụng radian

Vì vậy việc tính toán trong box2d sẽ khác với tính toán theo pixel do khác biệt về đơn vị nên sẽ cần phép chuyển đổi

### Chuyển hệ tọa độ box2d từ pixel sang hệ box2d và ngược lại

Do box2d sử dụng hệ tọa độ với đơn vị khác với cách vẽ của pixel với góc của pixel thường là top-left (x-right là hướng dương, y-bottom là hướng dương).

Việc chuyển đổi này theo hệ số. Điều này tùy thuộc vào pixel. Giả sử 50 pixel sẽ là 1 mét, để tính hệ số ta sử dụng quy tắc tam suất

50 pixel = 1 mét

1 pixel  = ? mét

1 * 1 / 50 = 0.02 mét

Vậy là ta đã có đc hệ số để quy đổi. Cứ 0.02 mét * xPixel thì sẽ đc xMeter
```
xMeters = 0.02f * xPixels;
yMeters = 0.02f * yPixels;
```

Đổi ngược lại từ mét sang pixel
```
xPixels = 50.0f * xMeters;
yPixels = 50.0f * yMeters;
```

### C#
Trong thư viện Velcro. Sử dụng static class ConvertUnits để chuyển đổi.

SetDisplayUnitToSimUnitRatio được sử dụng để set bao nhiêu pixel sang cho mỗi 1 mét.


## Các loại Box2d
---------
## World
Được sử dụng để cập nhật toàn bộ world của box2d. Do là thư việc nên việc tính toán sẽ qua world chịu trách nhiệm. Tại mỗi bước update ta chỉ việc gọi Step để cập nhật cho world

World cần phải có gravity nên cần đc set để áp dụng trong toàn bộ world

Do tôi sử dụng thư viện Velro dành riêng cho Monogame nên hệ thống tọa độ đã được đồng nhất với hệ tọa độ pixel. Lưu ý: chỉ đồng nhất trục x-right dương, y-bottom dương

```
// Đối với các thư viện khác thì gravity sẽ là -300 vì hệ tọa độ là bottom-left
World world = new World(new Vector2(0, 300))
```

## Body

Body là các điểm trong World. Không có hình dạng, để có các hình dạng phải thông qua Fixture
Body có 2 loại là BodyDef và Body

#### BodyDef

Được dùng để định nghĩa body. Việc định nghĩa bao gồm position, type, ...

Với type mặc định sẽ là Static

```
BodyDef groundDef = new BodyDef();
groundDef.Position = ConvertUnit.ToSimUnits(0, heightScreen/2 - 30);
```

#### Body

Là rigid body, đây là body sẽ được world update vì vậy cần pass BodyDef vào world để world có thể cập nhật. Sau mỗi lần update world ta sẽ hỏi vị trí của body trong world và convert qua pixel để vẽ lên màn hình

```
Body rigidBody = world.CreateBody(groundDef);
```

### Fixture

Thành phần sẽ attach hình dạng vào body.

Box2d định nghĩa các shape: polygon, edge, chain.

Việc định nghĩa shape sẽ cần width, height hoặc các vector hoặc các đỉnh

Sau khi định nghĩa xong sẽ attach vào body

```
// định nghĩa shape
PolygonShape box = new PolygonShape(1);
box.SetAsBox(ConvertUnits.ToSimUnit(20/2), ConvertUnits.ToSimUnit(10/2));
// attach shape vào body thông qua Fixture
Fixture fixture = rigidBody.CreateFixture(box);
```

box.SetAsBox sẽ sử dụng **1 nửa width và 1 nửa height** để tạo hình dạng. Trong trường hợp này sẽ box sẽ là 20 đơn vị cho width, và 10 đơn vị cho height

Lưu ý: Do sử dụng 1 nửa như vậy nên **điểm của body sẽ nằm ở giữa của box**. Khi vẽ pixel cần lưu ý điểm này bởi các công cụ vẽ sẽ vẽ từ điểm pixel đã cho (x, y) cộng width và height để ra texture. Nhưng vì point trả ra của rigid body là điểm nằm giữa của body nên cần phải cộng thêm vào

```
// position là vị trí vẽ box trên screen
position.X = ConvertUnits.ToDisplayUnits(rigidBody.Position.X) - width/2;
position.Y = ConvertUnits.ToDisplayUnits(rigidBody.Position.Y) - height/2;

```

### Giải quyết vấn đề vẽ hình từ box2d sang screen

Spin 360

```

```


## Các loại Body

Như tôi đã trình bày BodyDef khi định nghĩa mặc định sẽ là Static. Vậy Static Body là gì?

## Static

Vật đứng yên, ko di chuyển, chỉ tác động vào đối tượng khác và các đối tượng khác không thể tác động vào. Ví dụ như ground, ground là static và đứng yên nhưng các đối tượng khác tác động vào ground thì ground vẫn vậy ko bị ảnh hưởng


## Dynamic

Vật di chuyển, va chạm, có thể bị tác động vào và tác động qua các dynamic khác. Là thứ mà chúng ta muốn!

## Kinematic

Là sự kết hợp giữa static và dynamic. Vật có thể di chuyển nhưng ko bị tác động vào, có thể tác động vào vật khác. Ví dụ như platform, platform có thể di chuyển nhưng ta ko thể tác động vào nó

