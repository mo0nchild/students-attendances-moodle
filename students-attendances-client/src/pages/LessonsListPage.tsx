import AttendancesSheet from "@components/attendances/AttendancesSheet";
import LessonTable from "@components/lessontable/LessonTable";
import Loading from "@components/loading/Loading";
import PageCard from "@components/pagecard/PageCard";
import type { LessonInfoModel } from "@core/models/lesson-models";
import { useCourseStore, useLessonStore } from "@core/store/store";
import { BrushCleaning, LucideCircleArrowLeft, RefreshCcw } from "lucide-react";
import { useEffect, useMemo, useState, type JSX } from "react";
import { Alert, Button, Dropdown, Form, Stack } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function LessonsListPage(): JSX.Element {
	const { isLoading, fetchLessons, lessons, error, course, setCourse } = useLessonStore()
	const { courses, fetchCourses } = useCourseStore()
	const [ search, setSearch ] = useState(''); 

	const [ attendancesReport, setAttendancesReport ] = useState<number>()
	const navigate = useNavigate()

	const filteredLesson = useMemo(() => {
		try {
			const regex = new RegExp(search, 'i');
			return lessons.filter(lesson => regex.test(lesson.description));
		} catch (error) {
			console.error('Некорректное регулярное выражение:', error);
			return lessons
		}
	}, [lessons, search])

	const reloadPage = () => {
		if (!course) {
			navigate('/dashboard')
			return
		}
		fetchCourses().catch(error => console.log(error))
	}

	const handleLessonEdit = async (lesson: LessonInfoModel) => {
		navigate(`/lessons/edit/${lesson.externalId}`)
	}
	const handleLessonSelect = async (lesson: LessonInfoModel) => {
		navigate(`/lessons/attendance/${lesson.externalId}`)
	}

	useEffect(() => {
		(async () => {
			const currentCourse = courses.find(it => it.externalId == course?.externalId)
			if (!currentCourse) {
				navigate('/dashboard')
				return
			}
			setCourse(currentCourse)
			await fetchLessons(currentCourse.externalId)
		})().catch(error => console.log(error))
	// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [courses])

	// eslint-disable-next-line react-hooks/exhaustive-deps
	useEffect(() =>  reloadPage(), [])

	if (!course) return <></>
	return (
		<PageCard>
			<div className="p-2 py-4 p-md-4 shadow rounded bg-white bg-opacity-75">
				<Stack gap={2} className="justify-content-center">
					
					<div className='d-flex flex-row mb-4 justify-content-between align-items-center gap-4'>
						<div className='gradient-input-wrapper p-1' onClick={() => navigate('/dashboard')} style={{
									cursor: 'pointer'
								}}>
							<LucideCircleArrowLeft color='white' size={26}/>
						</div>
						<h3 className="m-0">Список уроков курса: {course.shortName}</h3>
					</div>
					<Form className="d-flex flex-row justify-content-center gap-3">
						<Form.Group controlId="search"
							style={{
								flexShrink: 0,
								flexGrow: 1
							}}
						>
							<Form.Label className="w-100 mx-2 text-start">Поиск по названию:</Form.Label>
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
							<Form.Label className="w-100 mx-2 text-start" style={{color: 'transparent'}}>a</Form.Label>
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
							<Form.Label className="w-100 mx-2 text-start" style={{color: 'transparent'}}>a</Form.Label>
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
					<div className="d-flex flex-row justify-content-center mb-2 gap-3">
						<div className="gradient-input-wrapper" style={{maxWidth: 'fit-content'}}>
							<Button style={{
								background: 'transparent',
								border: '0',
								fontSize: '13px'
							}} onClick={() => navigate(`/lessons/create`)}>
								Создать урок
							</Button>
						</div>
						<div className="gradient-input-wrapper" style={{width: 'fit-content'}}>
							<Dropdown onSelect={(eventKey) => {
								if (!eventKey) return
								setAttendancesReport(parseInt(eventKey))
							}}>
								<Dropdown.Toggle variant="light" className="h-100"
									style={{
										fontSize: '14px', 
										borderRadius: '10px'
									}}
								>
									Перейти к таблице
								</Dropdown.Toggle>

								<Dropdown.Menu>
									<Dropdown.Item eventKey={-1}>Общие занятия</Dropdown.Item>
									{
										course.groups.map((it, index) => (
											<Dropdown.Item key={index} eventKey={it.externalId}>{it.groupName}</Dropdown.Item>
										))
									}
								</Dropdown.Menu>
							</Dropdown>
							</div>
					</div>
					<div className="mb-2">
						{
							attendancesReport 
								? <AttendancesSheet onClose={() => setAttendancesReport(undefined)}
										groupId={attendancesReport < 0 ? null : attendancesReport}/>
								: <></>
						}
					</div>
					<Loading isLoading={isLoading}>
						<>
						<div className='d-flex flex-row justify-content-start align-items-center'>
							<h5 className="m-0">Список занятий:</h5>
						</div>
							{error && <Alert className="mb-2" variant="danger">{error}</Alert>}
							{ 
								filteredLesson.length > 0
									? <LessonTable lessons={filteredLesson} course={course} 
											onEdit={handleLessonEdit}
											onSelect={handleLessonSelect}
										/>
									: <div className="d-flex flex-column justify-content-center align-items-center mt-4">
											<p className="fs-4">Уроки не найдены</p> 
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
					
				</Stack>
			</div>
		</PageCard>
	)
}