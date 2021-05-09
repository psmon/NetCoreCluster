
# 닷넷코어 어플리케이션 클러스터화

닷넷코어 어플리케이션을 리눅스 도커환경에 클러스터화 하는 실험이며

최종목표는 단일지점 병목이 없는 분산처리 어플리케이션 작성입니다.

AKKA는 클러스터 기능사용을 위해 활용되었습니다.

## 부가목표

클러스터 어플리케이션의 최종목적은 다른 역활을 하는 닷넷코어 서비스를 각각 구동한후 하나의 어플리케이션처럼

작동하는것처럼 보이는 것입니다.

- 로컬에서 시작하는 클러스터 디버깅환경
- 도커컴포저를 통한 클러스터 디버깅환경구축
- 닷넷코어+도커 환경을 지배하여 쿠버및 클라우드 환경으로의 확장


## 사용모듈

```
<PackageReference Include="AkkaDotModule.Webnori" Version="1.1.2" />
```

Akka.net 1.4.12 가 사용되었으며, 위 통합패키징 모듈을 통해 실험되었습니다.


## 더 자세한 컨셉

링크 : http://wiki.webnori.com/display/webfr/NetCoreCluster


## 추가 참고

- https://github.com/petabridge/akkadotnet-bootstrap : 다양한 환경에 클러스터화


