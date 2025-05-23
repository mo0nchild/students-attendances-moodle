import Loading from "@components/loading/Loading";
import ModalWindow from "@components/modal/ModalWindow";
import PageCard from "@components/pagecard/PageCard";
import { useDeviceComm } from "@core/hooks/useDeviceComm";
import type { AttendanceTakenRequest, LessonInfoModel } from "@core/models/lesson-models";
import type { RfidMarkerModel, UserInfoWithGroupsModel } from "@core/models/user-models";
import { useLessonStore } from "@core/store/store";
import { coursesService } from "@services/CoursesService";
import { lessonService } from "@services/LessonService";
import { AxiosError } from "axios";
import { LucideCircleArrowLeft, RefreshCcw } from "lucide-react";
import { useEffect, useRef, useState, type JSX } from "react";
import { Alert, Badge, Button, Dropdown, Form, Stack, Table } from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";

function filterLessonName(name: string) {
  const filter = name.replace(/<\/?[^>]+(>|$)/g, '')
  return filter.length > 0 ? filter : 'Обычное занятие'
}

type StudentAttendance = {
	student: UserInfoWithGroupsModel
	attendance: string | undefined
}

const getBadgeColor = (attendance: string | undefined): string => {
	if (!attendance) return 'secondary'
  switch (attendance.toLocaleLowerCase()) {
    case 'п': return 'success';
    case 'о': return 'warning';
    case 'у': return 'primary';
		case 'н': return 'secondary';
    default: return 'secondary';
  }
};

export default function LessonAttendancePage(): JSX.Element {
	const { fetchLessons, error, course } = useLessonStore() // либо использовать shallow
	
	const [ lesson, setLesson ] = useState<LessonInfoModel>()
	const [ attendances, setAttendances ] = useState<StudentAttendance[]>([])
	const attendancesRef = useRef<StudentAttendance[]>([])
	const rfidsRef = useRef<RfidMarkerModel[]>([])
	const attendanceRequest = useRef<AttendanceTakenRequest>(null)

	const [ scanning, setScanning ] = useState<boolean>(false)
	const [ isLoading, setLoading ] = useState<boolean>(false)

	const [ notification, setNotification ] = useState<string>('')
	const [ selections, setSelections ] = useState<number[]>([])

	const { id } = useParams()
	const navigate = useNavigate()

	useEffect(() => console.log(selections), [selections])

	// eslint-disable-next-line react-hooks/exhaustive-deps
	useEffect(() => refreshPage(), [course, id])

	const refreshPage = () => {
		console.log('refresh')
		if (!id || !course) {
			navigate('/dashboard')
			return
		}
		(async () => {
			await fetchLessons(course!.externalId)
			const updatedLessons = useLessonStore.getState().lessons
			if (updatedLessons.length <= 0) return
			setLoading(true)
			const current = updatedLessons.find(it => it.externalId == parseInt(id))
			if (!current) {
				setLoading(false)
				return alert('Урок не найден')
			}
			setLesson(current)
			attendanceRequest.current = {
				lessonId: current.externalId,
				items: []
			}
			try {
				rfidsRef.current = (await coursesService.getRfidMarkers(course.externalId)).data
				const students = (await coursesService.getStudentsList(course.externalId)).data

				let attendancesList = [] as StudentAttendance[]
				if (current.groupInfo) {
					attendancesList = students.filter(it => it.groups.some(g => g.externalId == current.groupInfo!.externalId))
						.map(item => ({
							student: item,
							attendance: current.attendances!.find(it => it.studentId == item.externalId)?.acronym
						}) as StudentAttendance)
				} else {
					attendancesList = students.map(item => ({
						student: item,
						attendance: current.attendances!.find(it => it.studentId == item.externalId)?.acronym
					}) as StudentAttendance)
				} 
				setAttendances(attendancesList)
				attendancesRef.current = attendancesList
				setLoading(false)
			}
			catch (error) {
				if (error instanceof AxiosError) {
					alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
				}
				setLoading(false)
				console.log(error)
			}

		})().catch(error => {
			console.log(error)
			setLoading(false)
		})
	}

	const { isConnected } = useDeviceComm({
		enabled: scanning,
		onData: data => {
			if (!data) return
			if (attendanceRequest.current != null) {
				const rfidMarker = rfidsRef.current.find(item => item.rfidValue == data)

				const user = attendancesRef.current.find(it => it.student.externalId == rfidMarker?.userId)
				if (user) {
					const { firstName, lastName, externalId } = user.student
					setNotification(`Студент: ${firstName} ${lastName}`)
					attendanceRequest.current.items = [
						...attendanceRequest.current.items,
						{
							acronym: 'П',
							studentId: externalId
						}
					]
				}
				else setNotification(`Пропуск не найден`)
			}
			console.log(data)
		},
		onError: error => {
			console.log(error)
			if (error) alert(error)

			setScanning(false)
		}
	})

	const handleSetStatus = async (eventKey: string | null) => {
		if (!eventKey) return
		const acronym = (() => {
			switch (eventKey) {
				case '1': return 'П'
				case '2': return 'О'
				case '3': return 'У'
				case '4': return 'Н'
			}
		})()
		const request = {
			lessonId: parseInt(id!),
			items: selections.map(it => ({ acronym: acronym, studentId: it }))
		} as AttendanceTakenRequest
		try {
			const response = (await lessonService.setAttendances(request)).data
			console.log(response)
			navigate(`/processing/${response}`, {
				state: { prev: location.pathname }
			})
		}
		catch (error) {
			if (error instanceof AxiosError) {
				alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
			}
			console.log(error)
		}
	}

	const handleRfidAttendances = async () => {
		setScanning(false)
		if (!attendanceRequest.current) return
		try {
			const response = (await lessonService.setAttendances(attendanceRequest.current)).data
			console.log(response)
			navigate(`/processing/${response}`, {
				state: { prev: location.pathname }
			})
		}
		catch (error) {
			if (error instanceof AxiosError) {
				alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
			}
			console.log(error)
		}
	}

	if (!course || !lesson) return <></>
	return (
		<PageCard>
			<div className="p-2 py-4 p-md-4 shadow rounded bg-white bg-opacity-75">
				<Stack gap={2}>
					{error && <Alert className="mb-2" variant="danger">{error}</Alert>}
					<div className='d-flex flex-row mb-4 justify-content-start align-items-center gap-4'>
						<div className='gradient-input-wrapper p-1' onClick={() => navigate(`/lessons`)} style={{
							cursor: 'pointer'	
						}}>
							<LucideCircleArrowLeft color='white' size={26}/>
						</div>
						<h3 className="m-0">
							Посещения занятие: {<>{filterLessonName(lesson.description)}</>}
						</h3>
					</div>
					<div className='d-flex flex-row justify-content-end gap-2 mb-2'>
						<p className='m-0 shadow rounded bg-white bg-opacity-50 p-2 px-4'
							style={{
								fontSize: '16px'
							}}
						>
							Групповое занятие: &nbsp;
							<span style={{fontStyle: 'italic'}}>{lesson.groupInfo?.groupName ?? 'Без группы'}</span>
						</p>
						<div>
							<div className="gradient-input-wrapper">
								<Button onClick={() => refreshPage()} style={{
									backgroundColor: 'transparent', 
									border: '0'
								}}>
									<RefreshCcw size={16} color='white'/>
								</Button>
							</div>
						</div>
					</div>
					<div className="mb-2 d-flex flex-row justify-content-start gap-3
							shadow rounded bg-white bg-opacity-50 p-2 px-4 py-3">
						<div className="gradient-input-wrapper" style={{width: 'fit-content'}}>
							<Button onClick={() => setScanning(!scanning)} style={{
								backgroundColor: !isConnected ? 'transparent' : 'white', 
								border: '0',
								borderRadius: '10px',
								fontSize: '14px',
								color: !isConnected ? 'white' : 'black'
							}}>
								{ isConnected ? 'Сканирование...' : 'Сканировать пропуски' }
							</Button>
						</div>
						<div className="">
							<div className="gradient-input-wrapper" style={{width: 'fit-content'}}>
								<Dropdown onSelect={handleSetStatus}>
									<Dropdown.Toggle variant="light" className="h-100" disabled={selections.length == 0}
										style={{
											fontSize: '14px', 
											borderRadius: '10px'
										}}
									>
										Установить Статус
									</Dropdown.Toggle>

									<Dropdown.Menu>
										<Dropdown.Item eventKey={1}>Пришел на урок</Dropdown.Item>
										<Dropdown.Item eventKey={2}>Опоздал</Dropdown.Item>
										<Dropdown.Item eventKey={3}>Уваж. причина</Dropdown.Item>
										<Dropdown.Item eventKey={4}>Не пришел на урок</Dropdown.Item>
									</Dropdown.Menu>
								</Dropdown>
							</div>
						</div>
					</div>
					<div className='shadow rounded bg-white bg-opacity-75 p-2 px-4 py-4'>
						<h5 className='text-center'>Список посещений студентов:</h5>
						<Loading isLoading={isLoading}>
							<div className="rounded shadow-sm p-3 bg-white bg-opacity-75" style={{
								overflow: 'auto'
							}}>
								<Table responsive striped className="align-middle mb-0 w-100" style={{
									backgroundColor: 'transparent',
									overflow: 'auto'
								}}>
									<thead style={{fontSize: '15px', fontWeight: 'normal'}}>
										<tr>
											<th style={{}}>#</th>
											<th style={{}}>Имя</th>
											<th style={{}}>Фамилия</th>
											<th style={{}}>Email</th>
											<th style={{}}>Группы</th>
											<th style={{}}>Статус</th>
											<th style={{}}>
												<Form.Check
													inline
													onChange={(e) => {
														const checked = e.target.checked
														if (checked) {
															setSelections(attendances.map(item => item.student.externalId))
														}
														else setSelections([])
													}}
													label=""
													checked={attendances.length == selections.length}
													type={'checkbox'}
												/>

											</th>
										</tr>
									</thead>
									<tbody style={{fontSize: '14px'}}>
										{attendances.map(({ student, attendance }, index) => (
											<tr key={student.externalId}>
												<td>{index + 1}</td>
												<td>{student.firstName}</td>
												<td>{student.lastName}</td>
												<td>{student.email}</td>
												<td>
													<div>{
														student.groups.length > 0
															? student.groups.map((g, i) => (
																	<p className="m-0" key={`${student.externalId}-${i}`}>
																		{ g.groupName }
																	</p>
																))
															: <>Нет групп</>
													}</div>
												</td>
												<td>
													<Badge bg={getBadgeColor(attendance)}>{attendance ?? 'Н'}</Badge>
												</td>
												<td>
													<Form.Check
														inline
														onChange={(e) => {
															const checked = e.target.checked
															if (!checked) {
																setSelections(selections.filter(it => it != student.externalId))
															}
															else setSelections([...selections, student.externalId])
														}}
														label=""
														checked={selections.includes(student.externalId)}
														type={'checkbox'}
													/>
												</td>
											</tr>
										))}
									</tbody>
								</Table>
							</div>
						</Loading>
					</div>
				</Stack>
			</div>
			<ModalWindow onClose={() => setScanning(false)} isOpen={isConnected}>
				<div className='d-flex flex-column align-items-center px-5'>
					<div style={{ 
						width: '120px', 
						height: '100px', 
						overflow: 'hidden' 
					}}>
						<img src="/animation2.gif"
							alt="GIF" 
							style={{ 
								width: '100%', 
								height: 'auto', 
								objectFit: 'cover',
								objectPosition: 'top'
							}} 
						/>
						
					</div>
					<div className="d-flex flex-column gap-3">
						<p className='fs-5 m-0'>Сканирование...</p>
						{
							notification 
								? <p className='fs-6 m-0'>{ notification }</p>
								: <></>
						}
						<div className="gradient-input-wrapper">
							<Button className='bg-transparent border-0' 
								style={{
									fontSize: '14px',
									width: '90px'
								}} onClick={() => handleRfidAttendances()}>Подтвердить</Button>
						</div>
					</div>
				</div>
			</ModalWindow>
		</PageCard>
	)
}