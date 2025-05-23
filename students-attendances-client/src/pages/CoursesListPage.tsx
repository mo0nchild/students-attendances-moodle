import Loading from "@components/loading/Loading";
import PageCard from "@components/pagecard/PageCard";
import CourseTable from "@components/coursetable/CourseTable";
import { useCourseStore, useLessonStore } from "@core/store/store";
import { ArrowDownFromLine, ArrowUpToLine, BrushCleaning, RefreshCcw } from "lucide-react";
import { useEffect, useMemo, useState, type JSX } from "react";
import { Row, Container, Form, Col, Button, Alert, Dropdown } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

type OrderType = 'name' | 'time'
const orderTypeLabels: Record<OrderType, string> = {
  name: 'По названию',
  time: 'По времени',
};
function sortingHelper(a: string, b: string, desc: boolean) {
  const aValue = String(a).toLowerCase();
  const bValue = String(b).toLowerCase();
  if (aValue < bValue) return !desc ? -1 : 1;
  if (aValue > bValue) return !desc ? 1 : -1;
  return 0;
}

export default function CoursesListPage(): JSX.Element {
	const { error, courses, fetchCourses, isLoading  } = useCourseStore() 
  const { setCourse } = useLessonStore()
  const navigate = useNavigate()
  const [ order, setOrder ] = useState<OrderType>('time')
  const [ desc, setDesc ] = useState<boolean>(true)

	const [ search, setSearch ] = useState(''); 
  const filteredCourses = useMemo(() => {
    try {
      const regex = new RegExp(search, 'i');
      const filtered = courses.filter(course => regex.test(course.fullName));
      switch (order) {
        case 'name': return filtered.sort((a, b) => sortingHelper(a.shortName, b.shortName, desc))
        case 'time': return filtered.sort((a, b) => sortingHelper(a.startDate, b.startDate, desc))
      }
    } catch (error) {
      console.error('Некорректное регулярное выражение:', error);
      return courses
    }
  }, [courses, desc, order, search])

  const courseSelected = (id: number) => navigate(`/course/${id}`)
  const courseLessonCheck = (id: number) => {
    const selected = courses.find(it => it.externalId == id)
    if (selected) {
      setCourse(selected)
      navigate(`/lessons`)
    }
  }

  const reloadPage = () => {
    (async () => {
      await fetchCourses()
    })().catch(error => console.log(error))
  }
	// eslint-disable-next-line react-hooks/exhaustive-deps
	useEffect(() =>  reloadPage(), [])

  return (
    <PageCard>
      <Container fluid='xl' className="py-4 shadow-sm rounded bg-white bg-opacity-75">
        <h2 className="mb-4" style={{textTransform: 'uppercase'}}>Список ваших курсов</h2>
        <Row className="justify-content-center mb-2">
          <Col xs='12' md='12' lg='10'>
            <Form className="d-flex flex-row justify-content-center gap-3">
              <Form.Group controlId="search"
                style={{
                  width: '100%'
                }}
              >
                <Form.Label className="text-start">Поиск по названию:</Form.Label>
                <div className="gradient-input-wrapper mb-3">
                  <Form.Control
                    type="text"
                    className="gradient-input"
                    placeholder="Введите название курса"
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                  />
                </div>
              </Form.Group>
              <Form.Group controlId="clear-btn">
                <Form.Label className="text-start" style={{color: 'transparent'}}>a</Form.Label>
                <div className="gradient-input-wrapper mb-3">
                  <Button onClick={() => setSearch('')} style={{
                    backgroundColor: 'transparent', 
                    border: '0'
                  }}>
                    <BrushCleaning onClick={() => setSearch('')} size={20} color='white'/>
                  </Button>
                </div>
              </Form.Group>
              <Form.Group controlId="refresh-btn">
                <Form.Label className="text-start" style={{color: 'transparent'}}>a</Form.Label>
                <div className="gradient-input-wrapper mb-3">
                  <Button onClick={() => setSearch('')} style={{
                    backgroundColor: 'transparent', 
                    border: '0'
                  }}>
                    <RefreshCcw onClick={() => reloadPage()} size={20} color='white'/>
                  </Button>
                </div>
              </Form.Group>
            </Form>
          </Col>
        </Row>
        <Row className="justify-content-center mb-2">
          <Col xs='12' md='12' lg='10'>
            <div className="d-flex justify-content-start gap-2">
              <div className="gradient-input-wrapper">
                <Button onClick={() => setDesc(!desc)} style={{
                  border: '0', 
                  background: 'transparent',
                  height: '32px'
                }} >
                  { desc ? <ArrowDownFromLine size={16}/> : <ArrowUpToLine size={16}/> }
                </Button>
              </div>
              <Dropdown>
                <div className="gradient-input-wrapper" style={{}}>
                  <Dropdown.Toggle variant="success" style={{
                    background: 'white',
                    border: '0',
                    borderRadius: '10px',
                    height: '32px',
                    fontSize: '14px',
                    color: 'black'
                  }} id="dropdown-basic">
                    { orderTypeLabels[order] }
                  </Dropdown.Toggle>

                  <Dropdown.Menu >
                    <Dropdown.Item onClick={() => setOrder('time')}>По времени</Dropdown.Item>
                    <Dropdown.Item onClick={() => setOrder('name')}>По названию</Dropdown.Item>
                  </Dropdown.Menu>
                </div>
              </Dropdown>
              
            </div>
          </Col>
        </Row>
        <Row className="justify-content-center mb-5">
          <Col xs='12' md='12' lg='10'>
            <Loading isLoading={isLoading}>
              <>
                {error && <Alert className="mb-2" variant="danger">{error}</Alert>}
                {
                  filteredCourses.length > 0
                    ? <CourseTable courses={filteredCourses} onSelect={courseSelected} onLesson={courseLessonCheck}/>
                    : <div className="d-flex flex-column justify-content-center align-items-center mt-4">
                        <p className="fs-4">Курсы не найдены</p> 
                        <div className="gradient-input-wrapper mb-3 " style={{
                          maxWidth: '100px'
                        }}>
                          <Button onClick={() => reloadPage()} style={{
                            backgroundColor: 'transparent', 
                            border: '0',
                          }}>Еще раз</Button>
                        </div>
                      </div>
                }
              </>
            </Loading>
          </Col>
        </Row>
      </Container>
    </PageCard>
  );
} 