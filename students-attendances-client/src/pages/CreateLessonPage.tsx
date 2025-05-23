import PageCard from "@components/pagecard/PageCard";
import type { AttendanceModuleInfo } from "@core/models/course-models";
import type { GroupInfoModel } from "@core/models/group-models";
import type { CreateLessonModel } from "@core/models/lesson-models";
import { useLessonStore } from "@core/store/store";
import { lessonService } from "@services/LessonService";
import { AxiosError } from "axios";
import { LucideCircleArrowLeft } from "lucide-react";
import { useEffect, useState, type JSX } from "react";
import { Button, Col, Form, Row, Stack } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function CreateLessonPage(): JSX.Element {
	const { course } = useLessonStore()
	const navigate = useNavigate()
	
	const [ name, setName ] = useState<string>('')
	const [ attmodule, setModule ] = useState<AttendanceModuleInfo>()
	const [ group, setGroup ] = useState<GroupInfoModel>()
	const [ groupUsing, setGroupUsing ] = useState<boolean>(false)
	const [ lessonData, setLessonData ] = useState({
    date: getCurrentDate(),
    startTime: getCurrentTime(),
    endTime: getCurrentTime()
  });
	const handleDateTimeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setLessonData(prev => ({ ...prev, [name]: value }));
  };

	const handleCreateLesson = async () => {
		if (!isStartTimeBeforeOrEqualEndTime(lessonData.startTime, lessonData.endTime)) {
			return alert('Время окончания занятия меньше, чем время начала')
		}
		if (!attmodule) return alert('Модуль посещения не выбран')
		if (!course) return alert('Курс не найден')
		if (name.length <= 3) return alert('Название слишком короткое')

		if (attmodule.groupMode == 'Isolated' && group || attmodule.groupMode == 'None' && !group 
			|| attmodule.groupMode == 'Visible') {
			const request: CreateLessonModel = {
				startTime: toIsoString(lessonData.date, lessonData.startTime),
				endTime: toIsoString(lessonData.date, lessonData.endTime),
				attendanceId: attmodule.externalId,
				courseId: course.externalId,
				description: name,
				groupId: group?.externalId
			}
			try {
				const response = (await lessonService.createLesson(request)).data
				console.log(response)
				navigate(`/processing/${response}/-2`)
			}
			catch (error) {
				if (error instanceof AxiosError) {
					alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
				}
				console.log(error)
			}
		}
	}

	useEffect(() => {
		if (attmodule) setGroupUsing(attmodule.groupMode == 'Isolated' ? true : false)
	}, [attmodule])
	useEffect(() => {
		if (course) {
			setModule(course.attendanceModules[0])
			setGroup(undefined)
		}
	}, [course])
	
	if (!course) return <></>
	return (
		<PageCard>
			<Stack gap={1} className="p-2 py-4 p-md-4 shadow rounded bg-white bg-opacity-75">
				<div className='d-flex flex-row mb-4 justify-content-start align-items-center gap-4'>
					<div className='gradient-input-wrapper p-1' onClick={() => navigate(-1)} style={{
								cursor: 'pointer'
							}}>
						<LucideCircleArrowLeft color='white' size={26}/>
					</div>
					<h3 className="m-0 fs-3" style={{textTransform: 'uppercase'}}>Создание урока</h3>
				</div>
				<div>
					<h5 className="mb-3">Основная информация</h5>
					<Row className="justify-content-center">
						<Col sm='12' md='6' lg='4'>
							<Form.Group>
								<Form.Label className="w-100 mx-2 mb-0 text-start">Название урока:</Form.Label>
								<div className="gradient-input-wrapper mb-3">
									<Form.Control
										type="text"
										className="gradient-input"
										value={name}
										onChange={(e) => setName(e.target.value)}
										placeholder="Введите название урока"
									/>
								</div>
							</Form.Group>
						</Col>
						<Col sm='12' md='6' lg='4'>
							<Form.Group>
								<Form.Label className="w-100 mx-2 mb-0 text-start">Модуль:</Form.Label>
								<div className="gradient-input-wrapper custom-select-wrapper mb-3">
									<Form.Select className="gradient-input" aria-label="" style={{
										border: '0',
										background: 'white'
									}}
										defaultValue={attmodule?.externalId ?? 0} 
										onChange={(e) => {
											const selectedId = e.currentTarget.value
											const current = course.attendanceModules.find(it => parseInt(selectedId) == it.externalId)
											if (current) {
												setModule(current)
												setGroup(undefined)
											}
										}}
									>
										{
											course.attendanceModules.map((item, index) => (
												<option key={`module-${index}`} value={item.externalId}>{item.name}</option>
											))
										}
									</Form.Select>
								</div>
							</Form.Group>
						</Col>
					</Row>
				</div>
				<div className="mb-3">
					<Row className="justify-content-center gy-2">
						<Col sm='12' md='4' lg='3'>
							<Form.Label className="w-100 mx-2 mb-0 text-start"></Form.Label>
							<Form.Check
								className='text-start custom-switch'
								type="switch"
								id="custom-switch"
								label="Использовать группу"
								checked={groupUsing}
								onChange={(e) => {
									setGroupUsing(e.target.checked)
									if (!e.target.checked) setGroup(undefined)
								}}
								disabled={attmodule?.groupMode != 'Visible'}
							/>
						</Col>
						<Col  sm='12' md='8' lg='5'>
							<Form.Group>
								<Form.Label className="w-100 mx-2 mb-0 text-start">Настройка группы:</Form.Label>
								<div className={`gradient-input-wrapper custom-select-wrapper mb-3 ${groupUsing ? '' : 'disabled'}`}>
									<Form.Select className={`gradient-input custom-disabled-select`} aria-label="" 
										style={{
											border: '0',
											background: 'white'
										}} 
										value={group?.externalId ?? ''}
										disabled={!groupUsing}
										onChange={(e) => {
											const id = e.target.value
											const selectedGroup = course.groups.find(it => it.externalId == parseInt(id))
											if (selectedGroup) {
												setGroup(selectedGroup)
											}
										}}
									>
										<option value="" disabled>Выберите группу...</option>
										{
											course.groups.map((item, index) => (
												<option key={`group-${index}`} value={item.externalId}>{item.groupName}</option>
											))
										}
									</Form.Select>
								</div>
							</Form.Group>	
						</Col>
					</Row>
				</div>
				<div className="mb-4">
					<h6>Дата и время проведения занятия</h6>
					<Row className="justify-content-center">
						<Col xs='12' sm='12' md='6' lg='4'>
							<Form.Group className="mb-3">
								<Form.Label className="w-100 mx-2 mb-0 text-start">Дата занятия:</Form.Label>
								<Form.Control type="date" name="date" required
										value={lessonData.date}
              			onChange={handleDateTimeChange}
								/>
							</Form.Group>
						</Col>
						<Col xs='6' sm='6' md='3' lg='2'>
							<Form.Group className="mb-3">
								<Form.Label className="w-100 mx-2 mb-0">Начало</Form.Label>
								<Form.Control type="time" name="startTime" required 
									value={lessonData.startTime}
									onChange={handleDateTimeChange}
								/>
							</Form.Group>
						</Col>
						<Col xs='6' sm='6' md='3' lg='2'>
							<Form.Group className="mb-3">
								<Form.Label className="w-100 mx-2 mb-0">Конец</Form.Label>
								<Form.Control type="time" name="endTime" required
									value={lessonData.endTime}
									onChange={handleDateTimeChange}
								/>
							</Form.Group>
						</Col>
					</Row>
				</div>
				<div className='d-flex justify-content-center'>
					<div className="gradient-input-wrapper mb-3">
						<Button className='border-0 bg-transparent' style={{width: '200px'}}
							onClick={() => handleCreateLesson()}
						>
							Создать урок
						</Button>
					</div>
					
				</div>
			</Stack>
		</PageCard>
	)
}
const getCurrentDate = () => new Date().toISOString().split('T')[0] // YYYY-MM-DD

const getCurrentTime = () => new Date().toTimeString().slice(0, 5) // HH:MM

function isStartTimeBeforeOrEqualEndTime(startTime: string, endTime: string): boolean {
  const toMinutes = (time: string): number => {
    const [hours, minutes] = time.split(':').map(Number)
    return hours * 60 + minutes
  }
  return toMinutes(startTime) <= toMinutes(endTime)
}

const toIsoString = (date: string, time: string): string => {

  const iso = new Date(`${date}T${time}:00Z`).toISOString()
  return iso
}