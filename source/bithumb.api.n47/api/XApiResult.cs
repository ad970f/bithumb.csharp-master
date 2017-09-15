namespace Bithumb.API
{
    /// <summary>
    /// 웹서버로 부터의 수신 데이터를 담는 객체
    /// 모든 전문의 공통 응답이 포함된다.
    /// </summary>
    public class XApiResult
    {
        public XApiResult()
        {
            this.status = -1;
            this.message = ""; 
        }

        /// <summary>
        /// 결과 상태 코드 (정상 : 0000, 정상이외 코드는 에러 코드 참조)
        /// </summary>
        public int status
        {
            get;
            set;
        }

        /// <summary>
        /// 결과 메시지
        /// </summary>
        public string message
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 웹서버로 부터의 수신 데이터를 담는 객체
    /// 공통응답 + 전문 고유의 데이터가 추가된다. ( 제네릭 클래스 )
    /// </summary>
    public class ApiResult<T> : XApiResult
    {
        /// <summary>
        /// data
        /// </summary>
        public T data
        {
            get;
            set;
        }
    }
}